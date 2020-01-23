import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CookieService } from "ngx-cookie-service";
import { map } from "rxjs/operators";
import { INavigationItem, INavigationData } from "./left-navigation.interface";

@Injectable({
  providedIn: "root"
})
export class LeftNavigationService {
  readonly api = "ubaseline/api/IntranetNavigation";
  openingState: object;

  constructor(private http: HttpClient, private cookieService: CookieService) {}

  setOpeningState(item: INavigationItem) {
    this.openingState[item.id] = !item.isSelected;
    this.cookieService.set(
      "nav-opening-state",
      JSON.stringify(this.openingState)
    );
  }

  getNavigation(): Observable<INavigationItem[]> {
    return this.http.get<INavigationData>(this.api + `/LeftNavigation`).pipe(
      map(r => this.correctNestingLevel(r)),
      map(r => this.setOpenProperties(r))
    );
  }

  private correctNestingLevel(data: INavigationData): INavigationItem[] {
    return data.menuItems.map(item => {
      item.level = 0;
      return item;
    });
  }

  private setOpenProperties(data: INavigationItem[]): INavigationItem[] {
    const cookieData = this.cookieService.get("nav-opening-state");
    this.openingState = JSON.parse(cookieData);
    this.checkNavigationItem(data);
    return data;
  }

  private checkNavigationItem(data) {
    return data.map(item => {
      if (this.openingState.hasOwnProperty(item.id)) {
        item.isSelected = this.openingState[item.id];
      }
      if (item.children.length) {
        this.checkNavigationItem(item.children);
      }
    });
  }
}

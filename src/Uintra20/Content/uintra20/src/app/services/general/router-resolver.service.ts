import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class RouterResolverService {

  constructor( private router: Router) { }

  removePageRouter(url) {
    const baseUrl = url.baseUrl.replace(/\%2F/g, '').substr(1);
    this.router.config = this.router.config.filter(route => route.path !== baseUrl);
  }
}

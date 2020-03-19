import { Component, ViewEncapsulation, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { SafeHtml, DomSanitizer } from "@angular/platform-browser";
import ParseHelper from "src/app/shared/utils/parse.helper";
import { AddButtonService } from "src/app/ui/main-layout/left-navigation/components/my-links/add-button.service";
import {
  ISocialDetails,
  IUserTag,
  IMedia,
  IDocument
} from "src/app/feature/specific/activity/activity.interfaces";
import { ICommentData } from "src/app/feature/reusable/ui-elements/comments/comments.component";
import { ImageGalleryService } from "src/app/feature/reusable/ui-elements/image-gallery/image-gallery.service";
import { ILikeData } from "src/app/feature/reusable/ui-elements/like-button/like-button.interface";
import { EventSubscriptionService } from "../../../../feature/specific/activity/event-subscription/event-subscription.service";

@Component({
  selector: "event-details-page",
  templateUrl: "./event-details-page.html",
  styleUrls: ["./event-details-page.less"],
  encapsulation: ViewEncapsulation.None
})
export class EventDetailsPage implements OnInit {
  parsedData: any;
  data: any;
  details: ISocialDetails;
  tags: Array<IUserTag>;
  activityName: string;
  likeData: ILikeData;
  medias: Array<IMedia> = new Array<IMedia>();
  documents: Array<IDocument> = new Array<IDocument>();
  commentDetails: ICommentData;
  detailsDescription: SafeHtml;
  detailsTitle: SafeHtml;
  fullEventTime: Array<string>;
  subscribers: string[] = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private imageGalleryService: ImageGalleryService,
    private sanitizer: DomSanitizer,
    private addButtonService: AddButtonService,
    private router: Router,
    private eventSubscription: EventSubscriptionService,
  ) {
    this.activatedRoute.data.subscribe(data => {
      if (!data.requiresRedirect.get()) {
        this.data = data;
        this.addButtonService.setPageId(data.id);
      } else {
        this.router.navigate([data.errorLink.get().originalUrl.get()]);
      }
    });
  }

  get locationUrl() {
    return (
      "http://maps.google.co.uk/maps?q=" +
      this.parsedData.details.location.shortAddress
    );
  }

  public ngOnInit(): void {
    this.parsedData = ParseHelper.parseUbaselineData(this.data);
    this.eventSubscription.getListOfUsers(this.parsedData.details.id).subscribe((res: string[]) => {
      this.subscribers = res;
    })

    this.details = this.parsedData.details;
    this.commentDetails = {
      entityId: this.parsedData.details.id,
      entityType: this.parsedData.details.activityType
    };
    this.activityName = ParseHelper.parseActivityType(
      this.details.activityType
    );

    this.tags = Object.values(this.parsedData.tags);
    this.medias = Object.values(
      this.parsedData.details.lightboxPreviewModel.medias
    );
    this.documents = Object.values(
      this.parsedData.details.lightboxPreviewModel.otherFiles
    );
    this.fullEventTime = Object.values(
      this.parsedData.details.fullEventTime
    );

    this.detailsDescription = this.sanitizer.bypassSecurityTrustHtml(
      this.details.description
    );
    this.detailsTitle = this.sanitizer.bypassSecurityTrustHtml(
      this.details.headerInfo.title
    );
  }

  public openGallery(i) {
    const items = this.medias.map(el => ({
      src: el.url,
      w: el.width,
      h: el.height
    }));

    this.imageGalleryService.open(items, i);
  }

  toggleNotification(val: boolean) {
    this.eventSubscription.toggleNotification(this.parsedData.details.id, val).subscribe(res => {
      this.parsedData.details.isNotificationsDisabled = val;
    })
  }

  toggleSubscription() {
    (this.parsedData.details.isSubscribed
      ? this.eventSubscription.unsubscribe(this.parsedData.details.id)
      : this.eventSubscription.subscribe(this.parsedData.details.id))
    .subscribe((res: string[]) => {
      this.subscribers = res;
      this.parsedData.details.isSubscribed = !this.parsedData.details.isSubscribed
    });
  }
}

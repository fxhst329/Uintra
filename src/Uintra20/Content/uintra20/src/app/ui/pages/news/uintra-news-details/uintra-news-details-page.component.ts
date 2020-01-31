import { Component, ViewEncapsulation, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ILikeData } from "src/app/feature/project/reusable/ui-elements/like-button/like-button.interface";
import { ICommentData } from "src/app/feature/project/reusable/ui-elements/comments/comments.component";
import { SafeHtml, DomSanitizer } from "@angular/platform-browser";
import { ImageGalleryService } from "src/app/feature/project/reusable/ui-elements/image-gallery/image-gallery.service";
import ParseHelper from "src/app/feature/shared/helpers/parse.helper";
import {
  ISocialDetails,
  IMedia,
  IDocument
} from "./uintra-news-details-page.interface";
import { IUserTag } from "../../social/details/social-details.interface";

@Component({
  selector: "uintra-news-details-page",
  templateUrl: "./uintra-news-details-page.html",
  styleUrls: ["./uintra-news-details-page.less"],
  encapsulation: ViewEncapsulation.None
})
export class UintraNewsDetailsPage implements OnInit {
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
  constructor(
    private activatedRoute: ActivatedRoute,
    private imageGalleryService: ImageGalleryService,
    private sanitizer: DomSanitizer
  ) {
    this.activatedRoute.data.subscribe(data => (this.data = data));
  }

  public ngOnInit(): void {
    const parsedData = ParseHelper.parseUbaselineData(this.data);
    this.details = parsedData.details;
    this.commentDetails = {
      entityId: parsedData.details.id,
      entityType: parsedData.details.activityType
    };
    this.activityName = ParseHelper.parseActivityType(
      this.details.activityType
    );

    this.tags = Object.values(parsedData.tags);
    this.medias = Object.values(parsedData.details.lightboxPreviewModel.medias);
    this.documents = Object.values(
      parsedData.details.lightboxPreviewModel.otherFiles
    );

    this.likeData = {
      likedByCurrentUser: !!parsedData.likedByCurrentUser,
      id: parsedData.details.id,
      activityType: parsedData.details.activityType,
      likes: Object.values(parsedData.likes)
    };

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
}

import {
  Component,
  OnInit,
  Input,
  ViewChild,
  HostListener
} from "@angular/core";
import { DropzoneComponent } from "ngx-dropzone-wrapper";
import ParseHelper from "src/app/shared/utils/parse.helper";
import { ModalService } from "src/app/shared/services/general/modal.service";
import { HasDataChangedService } from "src/app/shared/services/general/has-data-changed.service";
import { MqService } from "src/app/shared/services/general/mq.service";
import { MAX_LENGTH } from "src/app/shared/constants/activity/activity-create.const";
import { ITagData } from "src/app/feature/reusable/inputs/tag-multiselect/tag-multiselect.interface";
import { IUserAvatar } from "src/app/feature/reusable/ui-elements/user-avatar/user-avatar-interface";
import { ActivityService } from "src/app/feature/specific/activity/activity.service";
import { ISocialCreateModel, IActivityCreatePanel } from "src/app/feature/specific/activity/activity.interfaces";
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: "app-social-create",
  templateUrl: "./social-create.component.html",
  styleUrls: ["./social-create.component.less"]
})
export class SocialCreateComponent implements OnInit {
  @Input() data: IActivityCreatePanel;
  @ViewChild("dropdownRef", { static: false }) dropdownRef: DropzoneComponent;
  @HostListener("window:beforeunload") doSomething() {
    return !this.hasDataChangedService.hasDataChanged;
  }
  @HostListener("window:resize", ["$event"])
  getScreenSize(event?) {
    this.deviceWidth = window.innerWidth;
  }
  deviceWidth: number;
  availableTags: Array<ITagData> = [];
  isPopupShowing = false;
  tags: Array<ITagData> = [];
  description = "";
  inProgress = false;
  userAvatar: IUserAvatar;
  panelData: any;
  linkPreviewId: number;
  //File it's array where file[0] is file's object generated by dropzone and file[1] is id
  files: Array<any> = [];

  get isSubmitDisabled() {
    if (
      ParseHelper.stripHtml(this.description).length > MAX_LENGTH ||
      this.inProgress
    ) {
      return true;
    }
    return !this.description && this.files.length === 0;
  }

  constructor(
    private socialContentService: ActivityService,
    private modalService: ModalService,
    private hasDataChangedService: HasDataChangedService,
    private mq: MqService,
    private translate: TranslateService,
  ) { }

  public ngOnInit(): void {
    this.panelData = ParseHelper.parseUbaselineData(this.data);
    this.availableTags = Object.values(this.panelData.tags);
    this.userAvatar = {
      name: this.panelData.creator.displayedName,
      photo: this.panelData.creator.photo
    };
    this.deviceWidth = window.innerWidth;
    this.getPlaceholder();
  }

  onShowPopUp() {
    if (this.panelData.canCreate) {
      this.showPopUp();
    }
  }
  onHidePopUp() {
    if (this.description || this.files.length) {
      if (confirm(this.translate.instant('common.AreYouSure'))) {
        this.resetForm();
        this.hidePopUp();
      }
    } else {
      this.resetForm();
      this.hidePopUp();
    }
  }

  hidePopUp() {
    this.modalService.removeClassFromRoot("disable-scroll");
    this.isPopupShowing = false;
    this.hasDataChangedService.reset();
  }

  showPopUp() {
    this.modalService.addClassToRoot("disable-scroll");
    this.isPopupShowing = true;
  }

  addAttachment() {
    this.dropdownRef.directiveRef.dropzone().clickableElements[0].click();
  }

  onUploadSuccess(fileArray: Array<any> = []): void {
    this.files.push(fileArray);
    this.hasDataChangedService.onDataChanged();
  }

  onFileRemoved(removedFile: object) {
    this.files = this.files.filter(file => {
      const fileElement = file[0];
      return fileElement !== removedFile;
    });
  }

  onTagsChange(e) {
    this.tags = e;
  }

  onDescriptionChange(e) {
    this.description = e;
    if (e) {
      this.hasDataChangedService.onDataChanged();
    }
  }

  getMediaIdsForResponse() {
    return this.files.map(file => file[1]).join(",");
  }
  getTagsForResponse() {
    return this.tags.map(tag => tag.id);
  }

  resetForm(): void {
    this.files = [];
    this.tags = [];
    this.description = "";
    this.linkPreviewId = null;
  }

  onSubmit() {
    this.inProgress = true;
    const requestModel: ISocialCreateModel = {
      description: this.description,
      ownerId: this.panelData.creator.id,
      newMedia: this.getMediaIdsForResponse(),
      tagIdsData: this.getTagsForResponse(),
      linkPreviewId: this.linkPreviewId
    };
    if (this.panelData.groupId) {
      requestModel.groupId = this.panelData.groupId;
    }
    this.socialContentService
      .submitSocialContent(requestModel)
      .then(response => {
        this.hidePopUp();
        this.socialContentService.refreshFeed();
      })
      .catch(err => {
        this.hidePopUp();
      })
      .finally(() => {
        this.inProgress = false;
        this.resetForm();
      });
  }

  canCreatePosts() {
    if (this.panelData) {
      return (
        this.panelData.canCreate ||
        this.panelData.createEventsLink ||
        this.panelData.createNewsLink
      );
    }
  }

  getPlaceholder() {
    return this.mq.isTablet(this.deviceWidth)
      ? 'socialsCreate.FormPlaceholder.lbl'
      : 'socialsCreate.MobileBtn.lbl';
  }

  addLinkPreview(linkPreviewId: number) {
    this.linkPreviewId = linkPreviewId;
  }
}

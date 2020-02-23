import { Component, OnInit, Input, ViewChild, HostListener } from '@angular/core';
import { IActivityCreatePanel } from '../../activity-create-panel.interface';
import { DropzoneComponent } from 'ngx-dropzone-wrapper';
import { ITagData } from 'src/app/feature/project/reusable/inputs/tag-multiselect/tag-multiselect.interface';
import { IUserAvatar } from 'src/app/feature/project/reusable/ui-elements/user-avatar/user-avatar-interface';
import ParseHelper from 'src/app/feature/shared/helpers/parse.helper';
import { ActivityService } from 'src/app/feature/project/specific/activity/activity.service';
import { ModalService } from 'src/app/services/general/modal.service';
import { MAX_LENGTH } from 'src/app/constants/activity/create/activity-create-const';
import { ISocialCreateModel } from 'src/app/feature/project/specific/activity/activity.interfaces';
import { HasDataChangedService } from 'src/app/services/general/has-data-changed.service';

@Component({
  selector: 'app-social-create',
  templateUrl: './social-create.component.html',
  styleUrls: ['./social-create.component.less']
})
export class SocialCreateComponent implements OnInit {
  @Input() data: IActivityCreatePanel;
  @ViewChild('dropdownRef', { static: false }) dropdownRef: DropzoneComponent;
  @HostListener('window:beforeunload') doSomething() {
    return !this.hasDataChangedService.hasDataChanged;
  }
  availableTags: Array<ITagData> = [];
  isPopupShowing = false;
  tags: Array<ITagData> = [];
  description = '';
  inProgress = false;
  userAvatar: IUserAvatar;
  panelData: any; // TODO change any type
  //File it's array where file[0] is file's object generated by dropzone and file[1] is id
  files: Array<any> = [];

  get isSubmitDisabled() {
    if (ParseHelper.stripHtml(this.description).length > MAX_LENGTH || this.inProgress) {
      return true;
    }
    return !this.description && this.files.length === 0;
  }

  constructor(
    private socialContentService: ActivityService,
    private modalService: ModalService,
    private hasDataChangedService: HasDataChangedService) { }

  public ngOnInit(): void {
    this.panelData = ParseHelper.parseUbaselineData(this.data);
    this.availableTags = Object.values(
      JSON.parse(JSON.stringify(this.data.tags.get().userTagCollection))
    );
    this.userAvatar = {
      name: this.panelData.creator.displayedName,
      photo: this.panelData.creator.photo
    };
  }

  onShowPopUp() {
    if(this.panelData.canCreate) {
      this.showPopUp();
    }
  }
  onHidePopUp() {
    if (this.description || this.files.length) {
      if (confirm('Are you sure?')) {
        this.resetForm();
        this.hidePopUp();
        this.hasDataChangedService.reset();
      }
    } else {
      this.resetForm();
      this.hidePopUp();
    }
  }

  hidePopUp() {
    this.modalService.removeClassFromRoot('disable-scroll');
    this.isPopupShowing = false;
  }

  showPopUp() {
    this.modalService.addClassToRoot('disable-scroll');
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
    debugger
    this.description = e;
    this.hasDataChangedService.onDataChanged();
  }

  getMediaIdsForResponse() {
    return this.files.map(file => file[1]).join(';');
  }
  getTagsForResponse() {
    return this.tags.map(tag => tag.id);
  }

  resetForm(): void {
    this.files = [];
    this.tags = [];
    this.description = '';
  }

  onSubmit() {
    this.inProgress = true;
    const requestModel: ISocialCreateModel = {
      description: this.description,
      ownerId: this.panelData.creator.id,
      newMedia: this.getMediaIdsForResponse(),
      tagIdsData: this.getTagsForResponse()
    };
    if (this.panelData.groupId) {requestModel.groupId = this.panelData.groupId}
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
      return this.panelData.canCreate || this.panelData.createNewsLink || this.panelData.createEventsLink;
    }
  }
}

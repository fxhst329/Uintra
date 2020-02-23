import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UbaselineCoreModule, AS_DYNAMIC_COMPONENT } from '@ubaseline/next';
import { RichTextEditorModule } from 'src/app/feature/project/reusable/inputs/rich-text-editor/rich-text-editor.module';
import { TagMultiselectModule } from 'src/app/feature/project/reusable/inputs/tag-multiselect/tag-multiselect.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SocialEditPageComponent } from './social-edit-page.component';
import { HttpClientModule } from '@angular/common/http';
import { MAX_LENGTH } from 'src/app/constants/activity/create/activity-create-const';
import { DropzoneWrapperModule } from 'src/app/feature/project/reusable/ui-elements/dropzone-wrapper/dropzone-wrapper.module';
import { GroupDetailsWrapperModule } from 'src/app/feature/project/specific/groups/group-details-wrapper/group-details-wrapper.module';
import { CanDeactivateGuard } from 'src/app/services/general/can-deactivate.service';

@NgModule({
  declarations:
    [
      SocialEditPageComponent
    ],
  imports:
    [
      CommonModule,
      RouterModule.forChild(
        [
          { path: '', component: SocialEditPageComponent, canDeactivate: [CanDeactivateGuard]}
        ]),
      UbaselineCoreModule,
      TagMultiselectModule,
      FormsModule,
      ReactiveFormsModule,
      DropzoneWrapperModule,
      HttpClientModule,
      GroupDetailsWrapperModule,
      RichTextEditorModule.configure({
        modules: {
          counter: {
            maxLength: MAX_LENGTH
          }
        }
      })
    ],
  providers:
    [
      { provide: AS_DYNAMIC_COMPONENT, useValue: SocialEditPageComponent }
    ],
  entryComponents:
    [
      SocialEditPageComponent
    ]
})
export class SocialEditPageModule { }


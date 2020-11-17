import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { UbaselineCoreModule } from 'ubaseline-next-for-uintra';
import { UintraGroupsCreatePage } from './uintra-groups-create-page.component';
import { CanDeactivateGuard } from 'src/app/shared/services/general/can-deactivate.service';
import { GroupsWrapperModule } from 'src/app/feature/specific/groups/groups-wrapper/groups-wrapper.module';
import { GroupsFormModule } from 'src/app/feature/specific/groups/groups-form/groups-form.module';
import { AuthenticatedLayoutModule } from 'src/app/ui/main-layout/authenticated-layout/authenticated-layout.module';

@NgModule({
  declarations: [UintraGroupsCreatePage],
  imports: [
    CommonModule,
    RouterModule.forChild([{path: "", component: UintraGroupsCreatePage, canDeactivate: [CanDeactivateGuard]}]),
    UbaselineCoreModule,
    GroupsWrapperModule,
    GroupsFormModule,
    AuthenticatedLayoutModule
  ],
  entryComponents: [UintraGroupsCreatePage]
})
export class UintraGroupsCreatePageModule {}

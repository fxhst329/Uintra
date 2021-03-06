import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { UbaselineCoreModule } from 'ubaseline-next-for-uintra';
import { NotificationsPage } from './notifications-page.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NavNotificationsModule } from 'src/app/feature/specific/nav-notifications/nav-notifications.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [NotificationsPage],
  imports: [
    CommonModule,
    RouterModule.forChild([{path: "", component: NotificationsPage}]),
    UbaselineCoreModule,
    NavNotificationsModule,
    InfiniteScrollModule,
    TranslateModule,
  ],
  entryComponents: [NotificationsPage]
})
export class NotificationsPageModule {}

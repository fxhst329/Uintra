export const pages = [
  {
    id: 'homePage',
    path: '__dynamic__',
    loadChildren: './ui/pages/home/home-page.module#HomePageModule'
  },
  {
    id: 'articlePage',
    path: '__dynamic__',
    loadChildren: './ui/pages/article/article-page.module#ArticlePageModule'
  },
  {
    id: 'notificationsPage',
    path: '__dynamic__',
    loadChildren: './ui/pages/notifications/notifications-page.module#NotificationsPageModule'
  },
  {
    id: 'socialDetailsPage',
    path: '__dynamic__',
    loadChildren: './ui/pages/social/details/social-details-page.module#SocialDetailsPageModule',
    cache: false
  },
  {
    id: 'socialEditPage',
    path: '__dynamic__',
    loadChildren: './ui/pages/social/edit/social-edit-page.module#SocialEditPageModule',
    cache: false
  },
];


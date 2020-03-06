export const panels = [
  {
    id: "centralFeedPanel",
    path: "__dynamic__",
    loadChildren:
      "./ui/panels/central-feed/central-feed-panel.module#CentralFeedPanelModule"
  },

  {
    id: "latestActivitiesPanel",
    path: "__dynamic__",
    loadChildren:
      "./ui/panels/latest-activities/latest-activities-panel.module#LatestActivitiesPanelModule"
  },

  {
    id: "likesPanel",
    path: "__dynamic__",
    loadChildren: "./ui/panels/likes/likes-panel.module#LikesPanelModule"
  },
  {
    id: "commentsPanel",
    path: "__dynamic__",
    loadChildren:
      "./ui/panels/comments/comments-panel.module#CommentsPanelModule"
  }
];

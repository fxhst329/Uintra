﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uintra.Core.Member.Abstractions;
using Uintra.Core.Member.Entities;
using Uintra.Core.Member.Services;
using Uintra.Features.Comments.Models;
using Uintra.Features.Comments.Services;
using Uintra.Features.Likes.Models;
using Uintra.Features.Likes.Services;
using Uintra.Features.LinkPreview.Models;
using Uintra.Features.Links;
using Uintra.Infrastructure.Extensions;

namespace Uintra.Features.Comments.Helpers
{
    public class CommentsHelper : ICommentsHelper
    {
        private readonly ICommentsService _commentsService;
        private readonly IProfileLinkProvider _profileLinkProvider;
        private readonly ILikesService _likesService;
        private readonly IIntranetMemberService<IntranetMember> _intranetMemberService;

        public CommentsHelper(
            ICommentsService commentsService,
            IIntranetMemberService<IntranetMember> intranetMemberService,
            IProfileLinkProvider profileLinkProvider,
            ILikesService likesService)
        {
            _commentsService = commentsService;
            _intranetMemberService = intranetMemberService;
            _profileLinkProvider = profileLinkProvider;
            _likesService = likesService;
        }

        public virtual async Task<CommentsOverviewModel> OverViewAsync(Guid activityId)
        {
            return await OverViewAsync(activityId, await _commentsService.GetManyAsync(activityId));
        }
        public virtual async Task<CommentsOverviewModel> OverViewAsync(Guid activityId, IEnumerable<CommentModel> comments, bool isReadOnly = false)
        {
            return new CommentsOverviewModel
            {
                ActivityId = activityId,
                Comments = await GetCommentViewsAsync(comments),
                ElementId = GetOverviewElementId(activityId),
                IsReadOnly = isReadOnly,
            };
        }
        public virtual IEnumerable<CommentViewModel> GetCommentViews(IEnumerable<CommentModel> comments)
        {
            var commentsList = comments.OrderBy(c => c.CreatedDate).ToArray();
            var currentMemberId = _intranetMemberService.GetCurrentMemberId();
            var creators = _intranetMemberService.GetAll().ToArray();
            var replies = commentsList.Where(_commentsService.IsReply);

            return commentsList
                    .Where(c => !_commentsService.IsReply(c))
                    .Select(comment =>
                    {
                        var model = GetCommentView(comment, currentMemberId, creators.SingleOrDefault(c => c.Id == comment.UserId));

                        var commentReplies = replies.Where(reply => reply.ParentId == model.Id);
                        model.Replies = commentReplies
                            .Select(reply =>
                                 GetCommentView(reply, currentMemberId, creators.SingleOrDefault(c => c.Id == reply.UserId)));

                        return model;
                    });
        }
        public virtual async Task<IEnumerable<CommentViewModel>> GetCommentViewsAsync(IEnumerable<CommentModel> comments)
        {
            var commentsList = comments.OrderBy(c => c.CreatedDate).ToArray();
            var currentMemberId = await _intranetMemberService.GetCurrentMemberIdAsync();
            var creators = (await _intranetMemberService.GetAllAsync()).ToArray();
            var replies = commentsList.Where(_commentsService.IsReply);

            return commentsList
                .Where(c => !_commentsService.IsReply(c))
                .Select(comment =>
                {
                    var model = GetCommentView(comment, currentMemberId,
                        creators.SingleOrDefault(c => c.Id == comment.UserId));

                    var commentReplies = replies.Where(reply => reply.ParentId == model.Id);
                    model.Replies = commentReplies
                        .Select(reply =>
                            GetCommentView(reply, currentMemberId,
                                creators.SingleOrDefault(c => c.Id == reply.UserId)));

                    return model;
                });
        }

        public virtual CommentViewModel GetCommentView(CommentModel comment, Guid currentMemberId, IIntranetMember creator)
        {
            var likes = _likesService.GetLikeModels(comment.Id).ToArray();
            var memberId = _intranetMemberService.GetCurrentMemberId();
            var model = comment.Map<CommentViewModel>();

            model.ModifyDate = _commentsService.WasChanged(comment) ? comment.ModifyDate.ToDateTimeFormat() : null;
            model.CanEdit = _commentsService.CanEdit(comment, currentMemberId);
            model.CanDelete = _commentsService.CanDelete(comment, currentMemberId);
            model.Creator = creator.ToViewModel();
            model.ElementOverviewId = GetOverviewElementId(comment.ActivityId);
            model.CommentViewId = _commentsService.GetCommentViewId(comment.Id);
            model.CreatorProfileUrl = creator == null ? null : _profileLinkProvider.GetProfileLink(creator);
            model.LinkPreview = comment.LinkPreview.Map<LinkPreviewModel>();
            model.LikedByCurrentUser = likes.Any(el => el.UserId == memberId);
            model.Likes = likes;
            model.LikeModel = GetLikesViewModel(comment.Id, memberId, likes);

            return model;
        }
        
        private static string GetOverviewElementId(Guid activityId)
        {
            return $"js-comments-overview-{activityId}";
        }
        private static LikesViewModel GetLikesViewModel(Guid commentId, Guid memberId, LikeModel[] likes)
        {
            return new LikesViewModel
            {
                EntityId = commentId,
                MemberId = memberId,
                Count = likes.Length,
                LikedByCurrentUser = likes.Any(el => el.UserId == memberId),
                Users = likes.Select(el => el.User)
            };
        }
    }
}
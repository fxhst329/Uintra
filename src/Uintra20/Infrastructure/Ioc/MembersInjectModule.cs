﻿using Compent.Shared.DependencyInjection.Contract;
using Uintra20.Core.Member;
using Uintra20.Core.Member.Entities;
using Uintra20.Core.Member.RelatedUser;
using Uintra20.Features.Tagging.UserTags.Services;

namespace Uintra20.Infrastructure.Ioc
{
	public class MembersInjectModule : IInjectModule
	{
		public IDependencyCollection Register(IDependencyCollection services)
		{
            //services.AddScoped(typeof(IIntranetMemberService<>), typeof(IntranetMemberService<>));
            services.AddScoped<IIntranetMemberService<IIntranetMember>, IntranetMemberService<IntranetMember>>();
            services.AddScoped<ICacheableIntranetMemberService, IntranetMemberService<IntranetMember>>();
            services.AddScoped<IIntranetUserService<IIntranetUser>, IntranetUserService<IntranetUser>>();
            services.AddScoped<IIntranetUserContentProvider, IntranetUserContentProvider>();
            services.AddScoped<IUserTagProvider, UserTagProvider>();
            services.AddScoped<IUserTagRelationService, UserTagRelationService>();
            services.AddScoped<IUserTagService, UserTagService>();

            return services;
		}
	}
}
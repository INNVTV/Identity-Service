﻿using Core.Application.Invitations.Models.Documents;
using Core.Application.Roles.Models.Documents;
using Core.Application.Users.Models.Documents;
using Core.Application.Users.Models.Views;
using Core.Domain.Entities;

namespace Core.Startup
{
    internal static class AutoMapperConfiguration
    {
        internal static void Configure()
        {
            AutoMapper.Mapper.Initialize(cfg => {

                // DocumentModels to Domain Entities:
                cfg.CreateMap<UserDocumentModel, User>();
                cfg.CreateMap<UserDocumentModel, UserListViewItem>();
                cfg.CreateMap<RoleDocumentModel, Role>();
                cfg.CreateMap<InvitationDocumentModel, Invitation>();

            });
        }
    }
}

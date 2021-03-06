﻿/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { ModuleWithProviders, NgModule } from '@angular/core';

import { SqxFrameworkModule } from 'framework';

import {
    AppFormComponent,
    AppClientsService,
    AppContributorsService,
    AppLanguagesService,
    AppsStoreService,
    AppsService,
    AppMustExistGuard,
    AuthService,
    ContentsService,
    DashboardLinkDirective,
    EventConsumersService,
    HelpComponent,
    HelpService,
    HistoryComponent,
    HistoryService,
    LanguageSelectorComponent,
    LanguageService,
    MustBeAuthenticatedGuard,
    MustBeNotAuthenticatedGuard,
    ResolveAppLanguagesGuard,
    ResolveContentGuard,
    ResolvePublishedSchemaGuard,
    ResolveSchemaGuard,
    SchemasService,
    UserManagementService,
    UsersProviderService,
    UsersService
} from './declarations';

@NgModule({
    imports: [
        SqxFrameworkModule
    ],
    declarations: [
        AppFormComponent,
        DashboardLinkDirective,
        HelpComponent,
        HistoryComponent,
        LanguageSelectorComponent
    ],
    exports: [
        AppFormComponent,
        DashboardLinkDirective,
        HelpComponent,
        HistoryComponent,
        LanguageSelectorComponent
    ]
})
export class SqxSharedModule {
    public static forRoot(): ModuleWithProviders {
        return {
            ngModule: SqxSharedModule,
            providers: [
                AppClientsService,
                AppContributorsService,
                AppLanguagesService,
                AppsStoreService,
                AppsService,
                AppMustExistGuard,
                AuthService,
                ContentsService,
                EventConsumersService,
                HelpService,
                HistoryService,
                LanguageService,
                MustBeAuthenticatedGuard,
                MustBeNotAuthenticatedGuard,
                ResolveAppLanguagesGuard,
                ResolveContentGuard,
                ResolvePublishedSchemaGuard,
                ResolveSchemaGuard,
                SchemasService,
                UserManagementService,
                UsersProviderService,
                UsersService
            ]
        };
    }
}
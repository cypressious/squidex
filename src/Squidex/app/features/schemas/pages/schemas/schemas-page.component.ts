/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';

import {
    AppComponentBase,
    AppsStoreService,
    AuthService,
    DateTime,
    fadeAnimation,
    ImmutableArray,
    MessageBus,
    ModalView,
    NotificationService,
    SchemaDto,
    SchemasService,
    UsersProviderService,
    Version
} from 'shared';

import { SchemaDeleted, SchemaUpdated } from './../messages';

@Component({
    selector: 'sqx-schemas-page',
    styleUrls: ['./schemas-page.component.scss'],
    templateUrl: './schemas-page.component.html',
    animations: [
        fadeAnimation
    ]
})
export class SchemasPageComponent extends AppComponentBase implements OnDestroy, OnInit {
    private messageUpdatedSubscription: Subscription;
    private messageDeletedSubscription: Subscription;

    public addSchemaDialog = new ModalView();

    public schemas = ImmutableArray.empty<SchemaDto>();
    public schemaQuery: string;
    public schemasFilter = new FormControl();
    public schemasFiltered = ImmutableArray.empty<SchemaDto>();

    constructor(apps: AppsStoreService, notifications: NotificationService, users: UsersProviderService,
        private readonly schemasService: SchemasService,
        private readonly authService: AuthService,
        private readonly messageBus: MessageBus,
        private readonly route: ActivatedRoute
    ) {
        super(notifications, users, apps);
    }

    public ngOnDestroy() {
        this.messageUpdatedSubscription.unsubscribe();
        this.messageDeletedSubscription.unsubscribe();
    }

    public ngOnInit() {
        this.schemasFilter.valueChanges
            .distinctUntilChanged()
            .debounceTime(100)
            .subscribe(q => {
                this.updateSchemas(this.schemas, this.schemaQuery = q);
            });

        this.route.params.map(q => q['showDialog'])
            .subscribe(showDialog => {
                if (showDialog) {
                    this.addSchemaDialog.show();
                }
            });

        this.messageUpdatedSubscription =
            this.messageBus.of(SchemaUpdated)
                .subscribe(m => {
                    this.updateSchemas(this.schemas.map(s => s.name === m.name ? updateSchema(s, this.authService, m) : s));
                });

        this.messageDeletedSubscription =
            this.messageBus.of(SchemaDeleted)
                .subscribe(m => {
                    this.updateSchemas(this.schemas.filter(s => s.name !== m.name));
                });

        this.load();
    }

    private load() {
        this.appName()
            .switchMap(app => this.schemasService.getSchemas(app).retry(2))
            .subscribe(dtos => {
                this.updateSchemas(ImmutableArray.of(dtos));
            }, error => {
                this.notifyError(error);
            });
    }

    public onSchemaCreated(dto: SchemaDto) {
        this.updateSchemas(this.schemas.push(dto), this.schemaQuery);

        this.addSchemaDialog.hide();
    }

    private updateSchemas(schemas: ImmutableArray<SchemaDto>, query?: string) {
        this.schemas = schemas;

        query = query || this.schemaQuery;

        if (query && query.length > 0) {
            schemas = schemas.filter(t => t.name.indexOf(query) >= 0);
        }

        schemas =
            schemas.sort((a, b) => {
                if (a.name < b.name) {
                    return -1;
                }
                if (a.name > b.name) {
                    return 1;
                }
                return 0;
            });

        this.schemasFiltered = schemas;
    }
}

function updateSchema(schema: SchemaDto, authService: AuthService, message: SchemaUpdated): SchemaDto {
    const me = `subject:${authService.user!.id}`;

    return new SchemaDto(
        schema.id,
        schema.name,
        message.label,
        message.isPublished,
        schema.createdBy, me,
        schema.created, DateTime.now(),
        new Version(message.version));
}


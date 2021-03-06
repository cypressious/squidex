/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import {
    AccessTokenDto,
    AppClientDto,
    AppClientsService,
    fadeAnimation,
    ModalView,
    NotificationService
} from 'shared';

const ESCAPE_KEY = 27;

@Component({
    selector: 'sqx-client',
    styleUrls: ['./client.component.scss'],
    templateUrl: './client.component.html',
    animations: [
        fadeAnimation
    ]
})
export class ClientComponent {
    public isRenaming = false;

    public token: AccessTokenDto;

    @Output()
    public renaming = new EventEmitter<string>();

    @Output()
    public revoking = new EventEmitter();

    @Input()
    public client: AppClientDto;

    @Input()
    public appName: string;

    public tokenDialog = new ModalView();

    public get clientName(): string {
        return this.client.name || this.client.id;
    }

    public get clientId(): string {
        return this.appName + ':' + this.client.id;
    }

    public get clientSecret(): string {
        return this.client.secret;
    }

    public renameForm: FormGroup =
        this.formBuilder.group({
            name: ['',
                Validators.required
            ]
        });

    constructor(
        private readonly appClientsService: AppClientsService,
        private readonly formBuilder: FormBuilder,
        private readonly notifications: NotificationService
    ) {
    }

    public resetForm() {
        this.renameForm.get('name').setValue(this.clientName);
    }

    public cancelRename() {
        this.isRenaming = false;
    }

    public startRename() {
        this.resetForm();

        this.isRenaming = true;
    }

    public onKeyDown(keyCode: number) {
        if (keyCode === ESCAPE_KEY) {
            this.cancelRename();
        }
    }

    public rename() {
        try {
            const newName = this.renameForm.get('name').value;

            if (newName !== this.clientName) {
                this.renaming.emit(newName);
            }
        } finally {
            this.isRenaming = false;
        }
    }

    public createToken(client: AppClientDto) {
        this.appClientsService.createToken(this.appName, client)
            .subscribe(dto => {
                this.token = dto;
                this.tokenDialog.show();
            }, error => {
                this.notifications.notify(error);
            });
    }
}


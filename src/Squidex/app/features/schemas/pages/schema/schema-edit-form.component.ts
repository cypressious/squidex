/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import {
    Notification,
    NotificationService,
    SchemasService,
    Version
} from 'shared';

import { SchemaPropertiesDto } from './schema-properties';

@Component({
    selector: 'sqx-schema-edit-form',
    styleUrls: ['./schema-edit-form.component.scss'],
    templateUrl: './schema-edit-form.component.html'
})
export class SchemaEditFormComponent implements OnInit {
    @Output()
    public saved = new EventEmitter<SchemaPropertiesDto>();

    @Output()
    public cancelled = new EventEmitter();

    @Input()
    public schema: SchemaPropertiesDto;

    @Input()
    public version: Version;

    @Input()
    public appName: string;

    public editFormSubmitted = false;
    public editForm: FormGroup =
        this.formBuilder.group({
            name: '',
            label: ['',
                [
                    Validators.maxLength(100)
                ]],
            hints: ['',
                [
                    Validators.maxLength(1000)
                ]]
        });

    constructor(
        private readonly schemas: SchemasService,
        private readonly formBuilder: FormBuilder,
        private readonly notifications: NotificationService
    ) {
    }

    public ngOnInit() {
        this.editForm.patchValue(this.schema);
    }

    public cancel() {
        this.reset();
        this.cancelled.emit();
    }

    public saveSchema() {
        this.editFormSubmitted = true;

        if (this.editForm.valid) {
            this.editForm.disable();

            const requestDto = this.editForm.value;

            this.schemas.putSchema(this.appName, this.schema.name, requestDto, this.version)
                .subscribe(dto => {
                    this.reset();
                    this.saved.emit(new SchemaPropertiesDto(this.schema.name, requestDto.label, requestDto.hints));
                }, error => {
                    this.editForm.enable();
                    this.notifications.notify(Notification.error(error.displayMessage));
                });
        }
    }

    private reset() {
        this.editFormSubmitted = false;
        this.editForm.reset();
    }
}
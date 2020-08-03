/* tslint:disable */
/* eslint-disable */
/**
 * Apex API
 * API for Apex
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
import {
    PlayerKind,
    PlayerKindFromJSON,
    PlayerKindFromJSONTyped,
    PlayerKindToJSON,
    PlayerStatus,
    PlayerStatusFromJSON,
    PlayerStatusFromJSONTyped,
    PlayerStatusToJSON,
} from './';

/**
 * 
 * @export
 * @interface PlayerDto
 */
export interface PlayerDto {
    /**
     * 
     * @type {number}
     * @memberof PlayerDto
     */
    readonly id: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerDto
     */
    readonly gameId: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerDto
     */
    readonly userId?: number | null;
    /**
     * 
     * @type {PlayerKind}
     * @memberof PlayerDto
     */
    kind: PlayerKind;
    /**
     * 
     * @type {string}
     * @memberof PlayerDto
     */
    readonly name: string;
    /**
     * 
     * @type {PlayerStatus}
     * @memberof PlayerDto
     */
    status: PlayerStatus;
    /**
     * 
     * @type {number}
     * @memberof PlayerDto
     */
    readonly colorId?: number | null;
    /**
     * 
     * @type {number}
     * @memberof PlayerDto
     */
    readonly startingRegion?: number | null;
    /**
     * 
     * @type {number}
     * @memberof PlayerDto
     */
    readonly startingTurnNumber?: number | null;
}

export function PlayerDtoFromJSON(json: any): PlayerDto {
    return PlayerDtoFromJSONTyped(json, false);
}

export function PlayerDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): PlayerDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'gameId': json['gameId'],
        'userId': !exists(json, 'userId') ? undefined : json['userId'],
        'kind': PlayerKindFromJSON(json['kind']),
        'name': json['name'],
        'status': PlayerStatusFromJSON(json['status']),
        'colorId': !exists(json, 'colorId') ? undefined : json['colorId'],
        'startingRegion': !exists(json, 'startingRegion') ? undefined : json['startingRegion'],
        'startingTurnNumber': !exists(json, 'startingTurnNumber') ? undefined : json['startingTurnNumber'],
    };
}

export function PlayerDtoToJSON(value?: PlayerDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'kind': PlayerKindToJSON(value.kind),
        'status': PlayerStatusToJSON(value.status),
    };
}



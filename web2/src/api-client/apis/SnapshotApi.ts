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


import * as runtime from '../runtime';
import {
    CreateSnapshotRequestDto,
    CreateSnapshotRequestDtoFromJSON,
    CreateSnapshotRequestDtoToJSON,
    SnapshotInfoDto,
    SnapshotInfoDtoFromJSON,
    SnapshotInfoDtoToJSON,
} from '../models';

export interface ApiGamesGameIdSnapshotsGetRequest {
    gameId: number;
}

export interface ApiGamesGameIdSnapshotsPostRequest {
    gameId: number;
    createSnapshotRequestDto?: CreateSnapshotRequestDto;
}

export interface ApiGamesGameIdSnapshotsSnapshotIdDeleteRequest {
    gameId: number;
    snapshotId: number;
}

export interface ApiGamesGameIdSnapshotsSnapshotIdLoadPostRequest {
    gameId: number;
    snapshotId: number;
}

/**
 * 
 */
export class SnapshotApi extends runtime.BaseAPI {

    /**
     */
    async apiGamesGameIdSnapshotsGetRaw(requestParameters: ApiGamesGameIdSnapshotsGetRequest): Promise<runtime.ApiResponse<Array<SnapshotInfoDto>>> {
        if (requestParameters.gameId === null || requestParameters.gameId === undefined) {
            throw new runtime.RequiredError('gameId','Required parameter requestParameters.gameId was null or undefined when calling apiGamesGameIdSnapshotsGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/games/{gameId}/snapshots`.replace(`{${"gameId"}}`, encodeURIComponent(String(requestParameters.gameId))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(SnapshotInfoDtoFromJSON));
    }

    /**
     */
    async apiGamesGameIdSnapshotsGet(requestParameters: ApiGamesGameIdSnapshotsGetRequest): Promise<Array<SnapshotInfoDto>> {
        const response = await this.apiGamesGameIdSnapshotsGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiGamesGameIdSnapshotsPostRaw(requestParameters: ApiGamesGameIdSnapshotsPostRequest): Promise<runtime.ApiResponse<SnapshotInfoDto>> {
        if (requestParameters.gameId === null || requestParameters.gameId === undefined) {
            throw new runtime.RequiredError('gameId','Required parameter requestParameters.gameId was null or undefined when calling apiGamesGameIdSnapshotsPost.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json-patch+json';

        const response = await this.request({
            path: `/api/games/{gameId}/snapshots`.replace(`{${"gameId"}}`, encodeURIComponent(String(requestParameters.gameId))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: CreateSnapshotRequestDtoToJSON(requestParameters.createSnapshotRequestDto),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => SnapshotInfoDtoFromJSON(jsonValue));
    }

    /**
     */
    async apiGamesGameIdSnapshotsPost(requestParameters: ApiGamesGameIdSnapshotsPostRequest): Promise<SnapshotInfoDto> {
        const response = await this.apiGamesGameIdSnapshotsPostRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async apiGamesGameIdSnapshotsSnapshotIdDeleteRaw(requestParameters: ApiGamesGameIdSnapshotsSnapshotIdDeleteRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.gameId === null || requestParameters.gameId === undefined) {
            throw new runtime.RequiredError('gameId','Required parameter requestParameters.gameId was null or undefined when calling apiGamesGameIdSnapshotsSnapshotIdDelete.');
        }

        if (requestParameters.snapshotId === null || requestParameters.snapshotId === undefined) {
            throw new runtime.RequiredError('snapshotId','Required parameter requestParameters.snapshotId was null or undefined when calling apiGamesGameIdSnapshotsSnapshotIdDelete.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/games/{gameId}/snapshots/{snapshotId}`.replace(`{${"gameId"}}`, encodeURIComponent(String(requestParameters.gameId))).replace(`{${"snapshotId"}}`, encodeURIComponent(String(requestParameters.snapshotId))),
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiGamesGameIdSnapshotsSnapshotIdDelete(requestParameters: ApiGamesGameIdSnapshotsSnapshotIdDeleteRequest): Promise<void> {
        await this.apiGamesGameIdSnapshotsSnapshotIdDeleteRaw(requestParameters);
    }

    /**
     */
    async apiGamesGameIdSnapshotsSnapshotIdLoadPostRaw(requestParameters: ApiGamesGameIdSnapshotsSnapshotIdLoadPostRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.gameId === null || requestParameters.gameId === undefined) {
            throw new runtime.RequiredError('gameId','Required parameter requestParameters.gameId was null or undefined when calling apiGamesGameIdSnapshotsSnapshotIdLoadPost.');
        }

        if (requestParameters.snapshotId === null || requestParameters.snapshotId === undefined) {
            throw new runtime.RequiredError('snapshotId','Required parameter requestParameters.snapshotId was null or undefined when calling apiGamesGameIdSnapshotsSnapshotIdLoadPost.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/games/{gameId}/snapshots/{snapshotId}/load`.replace(`{${"gameId"}}`, encodeURIComponent(String(requestParameters.gameId))).replace(`{${"snapshotId"}}`, encodeURIComponent(String(requestParameters.snapshotId))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiGamesGameIdSnapshotsSnapshotIdLoadPost(requestParameters: ApiGamesGameIdSnapshotsSnapshotIdLoadPostRequest): Promise<void> {
        await this.apiGamesGameIdSnapshotsSnapshotIdLoadPostRaw(requestParameters);
    }

}
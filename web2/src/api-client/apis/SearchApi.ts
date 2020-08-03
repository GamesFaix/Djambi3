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
    GamesQueryDto,
    GamesQueryDtoFromJSON,
    GamesQueryDtoToJSON,
    SearchGameDto,
    SearchGameDtoFromJSON,
    SearchGameDtoToJSON,
} from '../models';

export interface ApiSearchGamesPostRequest {
    gamesQueryDto?: GamesQueryDto;
}

/**
 * 
 */
export class SearchApi extends runtime.BaseAPI {

    /**
     */
    async apiSearchGamesPostRaw(requestParameters: ApiSearchGamesPostRequest): Promise<runtime.ApiResponse<Array<SearchGameDto>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json-patch+json';

        const response = await this.request({
            path: `/api/search/games`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: GamesQueryDtoToJSON(requestParameters.gamesQueryDto),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(SearchGameDtoFromJSON));
    }

    /**
     */
    async apiSearchGamesPost(requestParameters: ApiSearchGamesPostRequest): Promise<Array<SearchGameDto>> {
        const response = await this.apiSearchGamesPostRaw(requestParameters);
        return await response.value();
    }

}
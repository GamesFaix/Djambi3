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

/**
 * 
 * @export
 * @enum {string}
 */
export enum SelectionKind {
    NUMBER_1 = 1,
    NUMBER_2 = 2,
    NUMBER_3 = 3,
    NUMBER_4 = 4,
    NUMBER_5 = 5
}

export function SelectionKindFromJSON(json: any): SelectionKind {
    return SelectionKindFromJSONTyped(json, false);
}

export function SelectionKindFromJSONTyped(json: any, ignoreDiscriminator: boolean): SelectionKind {
    return json as SelectionKind;
}

export function SelectionKindToJSON(value?: SelectionKind | null): any {
    return value as any;
}


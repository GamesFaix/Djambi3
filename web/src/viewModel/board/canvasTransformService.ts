import * as MathJs from 'mathjs';
import Geometry from './geometry';
import { Point } from './model';

export interface CanvasTranformData {
    containerSize : Point,
    canvasMargin : number,
    contentPadding : number,
    regionCount : number,
    zoomLevel : number
}

export default class CanvasTransformService{
    //--- Transforms ---

    public static getBoardViewTransform(data : CanvasTranformData) : MathJs.Matrix {
        //Order is very important. Last transform gets applied to image first
        return Geometry.Transform.compose([
            CanvasTransformService.getTransformToCenterBoardInCanvas(data),
            CanvasTransformService.getTransformToScaleBoard(data),
        ]);
    }

    private static getTransformToScaleBoard(data: CanvasTranformData) : MathJs.Matrix {
        const scale = CanvasTransformService.getScale(data);
        return Geometry.Transform.scale({ x: scale, y: scale });
    }

    private static getTransformToCenterBoardInCanvas(data : CanvasTranformData) : MathJs.Matrix {
        //Boardviews start with their centroid at 0,0.
        const Point = Geometry.Point;

        const canvasSize = CanvasTransformService.getSize(data);

        let offset = Point.multiplyScalar(canvasSize, 0.5);

        let centroidToCenterOffset = Geometry.RegularPolygon.sideToCentroidOffsetFromCenterRatios(data.regionCount);
        centroidToCenterOffset = Point.multiplyScalar(centroidToCenterOffset, CanvasTransformService.getScale(data));

        offset = Point.add(offset, centroidToCenterOffset);

        return Geometry.Transform.translate(offset);
    }

    //------

    private static getCanvasContentAreaSizeWithNoZoom(containerSize : Point, canvasMargin : number) : Point {
        return Geometry.Point.subtractScalar(containerSize, 2 * canvasMargin);
    }

    private static getBoardPolygonBaseSize(regionCount : number) : Point {
        return Geometry.RegularPolygon.sideToSizeRatios(regionCount);
    }

    private static getTotalMarginSize(canvasMargin : number, contentPadding : number) : Point {
        const n = 2 * (canvasMargin + contentPadding);
        return { x: n, y: n };
    }

    private static getBoardSize(data : CanvasTranformData) : Point {
        let size = CanvasTransformService.getBoardPolygonBaseSize(data.regionCount);
        size = Geometry.Point.multiplyScalar(size, CanvasTransformService.getScale(data));
        size = Geometry.Point.add(size, this.getTotalMarginSize(data.canvasMargin, data.contentPadding));
        return size;
    }

    public static getSize(data : CanvasTranformData) : Point {
        const boardSize = CanvasTransformService.getBoardSize(data);
        return {
            x: Math.max(boardSize.x, data.containerSize.x),
            y: Math.max(boardSize.y, data.containerSize.y)
        };
    }

    //--- Scale

    public static getScale(data : CanvasTranformData) : number {
        return CanvasTransformService.getContainerSizeScaleFactor(
            data.containerSize,
            data.canvasMargin,
            data.contentPadding,
            data.regionCount
        )
            * CanvasTransformService.getZoomScaleFactor(data.zoomLevel);
    }

    public static getZoomScaleFactor(zoomLevel : number) : number {
        switch (zoomLevel) {
            //Increments of 0.1 from 0.5 to 2.0
            case -5: return 0.5;
            case -4: return 0.6;
            case -3: return 0.7;
            case -2: return 0.8;
            case -1: return 0.9;
            case  0: return 1.0;
            case  1: return 1.1;
            case  2: return 1.2;
            case  3: return 1.3;
            case  4: return 1.4;
            case  5: return 1.5;
            case  6: return 1.6;
            case  7: return 1.7;
            case  8: return 1.8;
            case  9: return 1.9;
            case 10: return 2.0;

            //Increments of 0.5 from 2.0 to 4.0
            case 11: return 2.5;
            case 12: return 3.0;
            case 13: return 3.5;
            case 14: return 4.0;

            default: throw `Unsupported zoom level: ${zoomLevel}`;
        }
    }

    public static minZoomLevel() { return -5; }

    public static maxZoomLevel() { return 14; }

    private static getContainerSizeScaleFactor(
        containerSize : Point,
        canvasMargin : number,
        contentPadding : number,
        regionCount : number
    ) : number {
        let contentAreaSize = CanvasTransformService.getCanvasContentAreaSizeWithNoZoom(containerSize, canvasMargin);
        contentAreaSize = Geometry.Point.subtractScalar(contentAreaSize, 2 * contentPadding);
        const boardBaseSize = CanvasTransformService.getBoardPolygonBaseSize(regionCount);
        return Geometry.Rectangle.largestScaleWithinBox(boardBaseSize, contentAreaSize);
    }

    //------
}
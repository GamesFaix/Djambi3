import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import Color from '../../boardRendering/color';
import { CellView } from '../../boardRendering/model';
import { Kernel as K } from '../../kernel';

export interface CanvasCellProps {
    cell: CellView,
    selectCell : (cell : CellView) => void
}

export default class CanvasCell extends React.Component<CanvasCellProps> {

    private getCellColor() : string {
        const baseColor = K.theme.getCellBaseColor(this.props.cell.type);
        const highlight = K.theme.getCellHighlight(this.props.cell.state);

        if (highlight === null) {
            return baseColor;
        } else {
            return Color.fromHex(baseColor)
                .lighten(highlight.intensity)
                .multiply(Color.fromHex(highlight.color))
                .toHex();
        }
    }

    private getBorderColor() : string {
        return K.theme.getCellBorderColor(this.props.cell.type);
    }

    public render() : JSX.Element {
        const color = this.getCellColor();
        let borderColor = this.getBorderColor();

        if (borderColor === null) {
            borderColor = color;
        }

        return (
            <CanvasPolygon
                polygon={this.props.cell.polygon}
                fillColor={color}
                strokeColor={borderColor}
                strokeWidth={1} //Stroke is necessary to fill gaps between polygons belonging to the same cell
                onClick={() => this.props.selectCell(this.props.cell)}
            />
        );
    }
}
namespace QrCodeGenerator;

public record Direction(int Row, int Column, MoveDirection NextDirection)
{
    public static implicit operator Direction((int x, int y, MoveDirection direction) pos) => new(pos.x, pos.y, pos.direction);
}

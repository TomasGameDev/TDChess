[System.Serializable]
public struct Vector3Byte
{
    public byte x;
    public byte y;
    public byte z;
    public Vector3Byte(byte x, byte y, byte z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public static Vector3Byte up
    {
        get { return new Vector3Byte(0, 1, 0); }
    }
    public static Vector3Byte zero
    {
        get { return new Vector3Byte(0, 0, 0); }
    }
    public static Vector3Byte operator +(Vector3Byte a, Vector3Byte b)
    {
        return new Vector3Byte((byte)(a.x + b.x), (byte)(a.y + b.y), (byte)(a.z + b.z));
    }
}
//[System.Serializable]
//public struct Vector3Int
//{
//    public byte x;
//    public byte y;
//    public byte z;
//    public Vector3Int(byte x, byte y, byte z)
//    {
//        this.x = x;
//        this.y = y;
//        this.z = z;
//    }
//    public static Vector3Int up
//    {
//        get { return new Vector3Int(0, 1, 0); }
//    }
//    public static Vector3Int zero
//    {
//        get { return new Vector3Int(0, 0, 0); }
//    }
//    public static Vector3Int operator +(Vector3Int a, Vector3Int b)
//    {
//        return new Vector3Int((byte)(a.x + b.x), (byte)(a.y + b.y), (byte)(a.z + b.z));
//    }
//    public static Vector3Int operator -(Vector3Int a, Vector3Int b)
//    {
//        return new Vector3Int((byte)(a.x - b.x), (byte)(a.y - b.y), (byte)(a.z - b.z));
//    }
//}
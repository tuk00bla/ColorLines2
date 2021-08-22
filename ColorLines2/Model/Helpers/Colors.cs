using System.Runtime.Serialization;

namespace BallsAndLines.Model
{
    [DataContract]
    public enum Color
    {
        [EnumMember]
        Red,
        [EnumMember]
        Green,
        [EnumMember]
        Yellow,
        [EnumMember]
        Pink,
        [EnumMember]
        Blue
    }
}

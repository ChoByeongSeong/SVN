using System;

public class YBEnum
{
    public enum eSortType
    {
        None = -1,
        Like,
        ClearCnt,
        Score,
        Date
    }

    public enum eUnitName
    {
        None = -1,
        Warrior,
        Defender,
        Charger,
        Archer,
        Catapult,
        Golem,
        Warlock,
        Minion,
    }

    public enum eUnitType
    {
        None = -1,
        Melee,
        Range,
        Heavy,
        Special
    }

    public enum eColorType
    {
        None = -1,
        Yellow,
        Green
    }

    public enum eProjectileType
    {
        None = -1,
        Arrow,
        Magic,
        Stone
    }

}

public static class UnitColor
{
    public static YBEnum.eColorType GetEnemyColorType(YBEnum.eColorType colorType)
    {
        switch (colorType)
        {
            case YBEnum.eColorType.Yellow:
                return YBEnum.eColorType.Green;

            case YBEnum.eColorType.Green:
                return YBEnum.eColorType.Green;
        }

        return YBEnum.eColorType.None;
    }

    public static YBEnum.eColorType PaseToEnum(string str)
    {
        YBEnum.eColorType unitType = (YBEnum.eColorType)Enum.Parse(typeof(YBEnum.eColorType), str);

        return unitType;
    }


    public static int PaseToInt(string str)
    {
        YBEnum.eColorType unitType = (YBEnum.eColorType)Enum.Parse(typeof(YBEnum.eColorType), str);

        return (int)unitType;
    }
}

public static class UnitName
{
    public static YBEnum.eUnitName ParseToEnum(string str)
    {
        YBEnum.eUnitName unitType = (YBEnum.eUnitName)Enum.Parse(typeof(YBEnum.eUnitName), str);

        return unitType;
    }

    public static int ParseToInt(string str)
    {
        YBEnum.eUnitName unitType = (YBEnum.eUnitName)Enum.Parse(typeof(YBEnum.eUnitName), str);

        return (int)unitType;
    }
}
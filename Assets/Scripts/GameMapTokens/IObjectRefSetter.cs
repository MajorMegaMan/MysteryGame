using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IObjectRefSetter
{
    void SetObjectRef(IObjectRefSetter target);

    IObjectRefSetter GetObjectRef();
}

interface IGameMapTileSetter
{
    void SetTile(GameMapTile target);

    GameMapTile GetTile();

    IGameMapTokenSetter GetITile();

    int GetID();
}

interface IGameMapTokenSetter
{
    void SetToken(GameMapToken target);

    void SetToken(GameMapToken target, int tokenID);

    void ClearToken(int tokenID);

    GameMapToken GetToken(int tokenID);
}

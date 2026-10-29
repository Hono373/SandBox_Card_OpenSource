using UnityEngine;

public interface IAssetLoad
{
    T Get<T>(string name);
    T Get<T>(string name, bool v);
    T GetSo<T>(string v1, bool v2);
    Sprite GetSpriteSync(string v);
}

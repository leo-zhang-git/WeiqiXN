using UnityEngine;

public abstract class EntityWithGO : EntityBase
{
    public GameObject gameObject;
    public Transform transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    protected EntityWithGO(SceneBase scene, string guid, GameObject gameObject) : base(scene, guid)
    {
        this.gameObject = gameObject;
    }
}

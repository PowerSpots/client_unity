using System.Collections;
using UnityEngine;

public enum LinkConstraintType
{
    LC_PositionXYZ,
    LC_PositionXY,
    LC_PositionXZ,
    LC_PositionYZ
}

[ExecuteInEditMode]
public class LinkConstraint : MonoBehaviour 
{
    public Transform target;
    public Vector3 offset = Vector3.zero;
    public LinkConstraintType constraintType = LinkConstraintType.LC_PositionXZ;
    public bool positionOnly = true;
    public string targetName = "Bip001";

    void Start()
    {
        FindTarget();
        LateUpdate();
    }
    
    void LateUpdate() 
    {
        if (target != null) 
        {
            switch (constraintType)
            {
                case LinkConstraintType.LC_PositionXY:
                {
                    transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
                }
                break;

                case LinkConstraintType.LC_PositionXZ:
                {
                    transform.position = new Vector3(target.position.x, transform.position.y, target.position.z) + offset;
                }
                break;

                case LinkConstraintType.LC_PositionYZ:
                {
                    transform.position = new Vector3(transform.position.x, target.position.y, target.position.z) + offset;
                }
                break;

                default:
                {
                    transform.position = target.position + offset;
                }
                break;
            }

            if (positionOnly == false)
            {
                transform.rotation = target.rotation;
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }
#if UNITY_EDITOR
        else
        {
            FindTarget();
        }
#endif
    }

    public void FindTarget() 
    {
        if (string.IsNullOrEmpty(targetName) || target != null) 
            return;

        var node = transform.parent.FindDeepChild(targetName);
        if (node != null) 
        {
            target = node;
        }
    }
}
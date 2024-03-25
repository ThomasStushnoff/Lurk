using Entities;
using UnityEngine;

namespace Interfaces
{
    public interface IGrabbable
    {
        void PickUp(BaseEntity entity);
        void Drop();
        void Place(Vector3 position, Quaternion rotation);
    }
}
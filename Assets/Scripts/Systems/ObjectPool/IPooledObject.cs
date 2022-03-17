using System.Collections;
using System.Collections.Generic;

public interface IPooledObject
{
    void SetIsActiveInPool(bool isActive);
}
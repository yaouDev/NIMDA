
public interface IItemContainer
{
    bool ContainItem(object item);
    bool RemoveItem(object item);
    bool AddItem(object item);
    bool IsFull();
}

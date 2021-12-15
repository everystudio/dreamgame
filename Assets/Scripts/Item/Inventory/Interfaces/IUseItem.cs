public interface IUseItem
{
    void OnUseItem(int _iIndex, ItemData _data, int _iAmount, int _iReduce = 1);
}
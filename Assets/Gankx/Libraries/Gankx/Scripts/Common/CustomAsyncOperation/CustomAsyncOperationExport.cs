using Gankx;

public static class CustomAsyncOperationExport
{
    public static uint INVALID_ID = CustomAsyncOperationService.InvalidId;

    public static void Cancel(uint id)
    {
        CustomAsyncOperationService.instance.Remove(id);
    }

    public static bool GetIsDone(uint id)
    {
        return CustomAsyncOperationService.instance.GetIsDone(id);
    }

    public static float GetProgress(uint id)
    {
        return CustomAsyncOperationService.instance.GetProgress(id);
    }
}

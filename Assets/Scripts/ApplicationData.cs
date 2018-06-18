public class ApplicationData {
    public static int playerNumber = 0;

    public static bool IsInMultiPlayerMode()
    {
        return playerNumber > 1;
    }
}

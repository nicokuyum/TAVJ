
public class Compressor
{   
    private static int GetBitsRequired(long value) {
        int bitsRequired = 0;
        while (value > 0) {
            bitsRequired++;
            value >>= 1;
        }
        return bitsRequired;
    }
    
    

}

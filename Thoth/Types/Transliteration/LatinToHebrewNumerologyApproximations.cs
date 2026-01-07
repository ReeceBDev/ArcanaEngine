namespace Thoth.Types.Transliteration
{
    internal enum LatinToHebrewNumerologyApproximations
    {
        Error = 0,
        HardSoftConflictError = 404,
        A = 1,
        B = 2,
        C = 404, // HardSoftConflictError = must be handled correctly! This must become either Z or K.
        Ch = 8, // Yes, there are double letters, just to spice things up even further!
        D = 4,
        E = 5,
        F = 80,
        G = 3,
        H = 8,
        I = 10,
        J = 10,
        K = 20,
        L = 30,
        M = 40,
        N = 50,
        O = 70,
        P = 80,
        Q = 100,
        Qu = (K + W),
        R = 200,
        S = 60,
        Sh = 300,
        Th = 400,
        T = 9,
        U = 6,
        V = 6,
        W = 6,
        X = (K + S),
        Y = 10,
        Z = 90,
    }
}

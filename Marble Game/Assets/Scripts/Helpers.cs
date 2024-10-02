using UnityEngine;

//THIS THING HOUSES STATIC METHODS THAT MIGHT BE NECESSARY IN MORE PLACES THAN ONE
public static class Helpers
{
    public static float Map(float value, float originalMin, float originalMax, float newMin,
    float newMax, bool clamp = false)
    {
        float val = newMin + (newMax - newMin) * ((value - originalMin) / (originalMax - originalMin));

        return clamp ? Mathf.Clamp(val, Mathf.Min(newMin, newMax), Mathf.Max(newMin, newMax)) : val;
    }

    //returns the distance of 
    public static float Project(Vector2 projectTo, Vector2 projection)
    {
        return Vector2.Dot(projectTo, projection) / projectTo.magnitude;
    }
}
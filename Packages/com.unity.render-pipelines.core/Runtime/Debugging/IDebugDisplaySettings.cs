using System;
using UnityEngine;

namespace UnityEngine.Rendering
{
    /// <summary>
    /// Interface for storing the debug settings
    /// </summary>
    public interface IDebugDisplaySettings
    {
        /// <summary>
        /// Reset the stored debug settings
        /// </summary>
        void Reset();

        /// <summary>
        /// Executes an action for each element
        /// </summary>
        /// <param name="onExecute"></param>
        void ForEach(Action<IDebugDisplaySettingsData> onExecute);

        /// <summary>
        /// Adds a <see cref="IDebugDisplaySettingsData"/> to this instance of <see cref="IDebugDisplaySettings"/>
        /// </summary>
        /// <param name="newData">The <see cref="IDebugDisplaySettingsData"/> to be added to this settings</param>
        /// <returns><see cref="IDebugDisplaySettingsData"/></returns>
        IDebugDisplaySettingsData Add(IDebugDisplaySettingsData newData)
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Thoth.Types.ThothArcana;

namespace Thoth.Types
{
    /// <summary> Collection of celestial correspondencies which relate to each of the celestial bodies. </summary>
    internal interface ICelestialCorrespondences
    {
        /// <summary> The primary sun sign's zodiacal correspondence. Represents the internal, decribes the practitioner's inner characteristics. </summary>
        IZodiacalArcanaCorrespondence ZodiacalSunSign { get; }

        /// <summary> The secondary rising sun sign's zodiacal correspondence. Represents the external, how a practitioner behaves and is percieved by others. </summary>
        IZodiacalArcanaCorrespondence? RisingSunSign { get; }

        /// <summary> The tetietary sign's zodiacal correspondence. Contrasts the sun. May present lacking characteristics for a practitioner. </summary>
        IZodiacalArcanaCorrespondence? MoonSign { get; }

        /// <summary> Mercury's zodiacal correspondence. </summary>
        IZodiacalArcanaCorrespondence? MercurySign { get; }

        /// <summary> Venus' zodiacal correspondence. </summary>
        IZodiacalArcanaCorrespondence? VenusSign { get; }

        /// <summary> The zodiacal correspondence of Mars. </summary>
        IZodiacalArcanaCorrespondence? MarsSign { get; }

        /// <summary> The zodiacal correspondence of Jupiter. </summary>
        IZodiacalArcanaCorrespondence? JupiterSign { get; }

        /// <summary> The zodiacal correspondence of Saturn. </summary>
        IZodiacalArcanaCorrespondence? SaturnSign { get; }
    }
}

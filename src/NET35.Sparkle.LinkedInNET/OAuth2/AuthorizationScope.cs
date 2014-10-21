
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;

    /// <summary>
    /// Permissions for authorization requests.
    /// </summary>
    [Flags]
    public enum AuthorizationScope : ushort
    {
        /// <summary>
        /// The read basic profile (r_basicprofile).
        /// Name, photo, headline, and current positions.
        /// </summary>
        ReadBasicProfile = 0x001,

        /// <summary>
        /// The read full profile (r_fullprofile).
        /// Full profile including experience, education, skills, and recommendations.
        /// </summary>
        ReadFullProfile = 0x002,

        /// <summary>
        /// The read email address (r_emailaddress).
        /// The primary email address you use for your LinkedIn account
        /// </summary>
        ReadEmailAddress = 0x004,

        /// <summary>
        /// The read network (r_network).
        /// Your 1st and 2nd degree connections.
        /// </summary>
        ReadNetwork = 0x008,

        /// <summary>
        /// The read contact information (r_contactinfo).
        /// Address, phone number, and bound accounts.
        /// </summary>
        ReadContactInfo = 0x010,

        /// <summary>
        /// The read write network updates (rw_nus).
        /// Retrieve and post updates to LinkedIn as you.
        /// </summary>
        ReadWriteNetworkUpdates = 0x020,

        /// <summary>
        /// The read write company page (rw_company_admin).
        /// Edit company pages for which I am an Admin and post status updates on behalf of those companies.
        /// </summary>
        ReadWriteCompanyPage = 0x040,

        /// <summary>
        /// The read write groups (rw_groups).
        /// Retrieve and post group discussions as you.
        /// </summary>
        ReadWriteGroups = 0x080,

        /// <summary>
        /// The write messages (w_messages).
        /// Send messages and invitations to connect as you.
        /// </summary>
        WriteMessages = 0x100,
    }
}

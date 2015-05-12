
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;

    /// <summary>
    /// Permissions for authorization requests.
    /// </summary>
    [Flags]
    public enum AuthorizationScope : int
    {
        /// <summary>
        /// Read basic profile (r_basicprofile).
        /// Name, photo, headline, and current positions.
        /// </summary>
        ReadBasicProfile = 0x001,

        /// <summary>
        /// Read full profile (r_fullprofile).
        /// Full profile including experience, education, skills, and recommendations.
        /// </summary>
        ReadFullProfile = 0x002,

        /// <summary>
        /// Read email address (r_emailaddress).
        /// The primary email address you use for your LinkedIn account
        /// </summary>
        ReadEmailAddress = 0x004,

        /// <summary>
        /// Read network (r_network).
        /// Your 1st and 2nd degree connections.
        /// </summary>
        ReadNetwork = 0x008,

        /// <summary>
        /// Read contact information (r_contactinfo).
        /// Address, phone number, and bound accounts.
        /// </summary>
        ReadContactInfo = 0x010,

        /// <summary>
        /// Read write network updates (rw_nus).
        /// Retrieve and post updates to LinkedIn.
        /// </summary>
        ReadWriteNetworkUpdates = 0x020,

        /// <summary>
        /// Read write company page (rw_company_admin).
        /// Edit company pages for which I am an Admin and post status updates on behalf of those companies.
        /// </summary>
        ReadWriteCompanyPage = 0x040,

        /// <summary>
        /// Read write groups (rw_groups).
        /// Retrieve and post group discussions.
        /// </summary>
        ReadWriteGroups = 0x080,

        /// <summary>
        /// Write messages (w_messages).
        /// Send messages and invitations to connect.
        /// </summary>
        WriteMessages = 0x100,

        /// <summary>
        /// Share (w_share).
        /// Post a comment that includes a URL to the content you wish to share.
        /// Share with specific values — You provide the title, description, image, etc.
        /// </summary>
        /// <remarks>
        /// https://developer.linkedin.com/docs/share-on-linkedin
        /// </remarks>
        WriteShare = 0x200,
    }
}

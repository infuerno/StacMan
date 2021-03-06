// <auto-generated>
//     This file was generated by a T4 template.
//     Don't change it directly as your change would get overwritten. Instead, make changes
//     to the .tt file (i.e. the T4 template) and save it to regenerate this file.
// </auto-generated>

using System;

namespace StackExchange.StacMan
{
    /// <summary>
    /// StacMan RelatedSite, corresponding to Stack Exchange API v2's related_site type
    /// http://api.stackexchange.com/docs/types/related-site
    /// </summary>
    public partial class RelatedSite : StacManType
    {
        /// <summary>
        /// api_site_parameter
        /// </summary>
        [Field("api_site_parameter")]
        public string ApiSiteParameter { get; internal set; }

        /// <summary>
        /// name
        /// </summary>
        [Field("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// relation
        /// </summary>
        [Field("relation")]
        public string Relation { get; internal set; }

        /// <summary>
        /// site_url
        /// </summary>
        [Field("site_url")]
        public string SiteUrl { get; internal set; }

    }
}

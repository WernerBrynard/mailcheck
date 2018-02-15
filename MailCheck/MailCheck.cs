using MailCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailCheck
{
    public class MailCheck
    {
        public int DomainThreshold = 3;
        public int SecondLevelThreshold = 3;
        public int TopLevelThreshold = 3;

        public string[] DefaultDomains = new string[] { "msn.com", "bellsouth.net", "telus.net", "comcast.net", "optusnet.com.au", "earthlink.net", "qq.com", "sky.com", "icloud.com", "mac.com", "sympatico.ca", "googlemail.com", "att.net", "xtra.co.nz", "web.de", "cox.net", "gmail.com", "ymail.com", "aim.com", "rogers.com", "verizon.net", "rocketmail.com", "google.com", "optonline.net", "sbcglobal.net", "aol.com", "me.com", "btinternet.com", "charter.net", "shaw.ca" };

        public string[] DefaultSecondLevelDomains = new string[] { "yahoo", "hotmail", "live", "outlook" };

        public string[] DefaultTopLevelDomains = new string[] { "com", "com.au", "com.tw", "ca", "co.za", "co.nz", "co.uk", "de", "fr", "it", "ru", "net", "org", "edu", "gov", "jp", "nl", "kr", "se", "eu", "ie", "co.il", "us", "at", "be", "dk", "hk", "es", "gr", "ch", "no", "cz", "in", "net", "net.au", "info", "biz", "mil", "co.jp", "sg", "hu", "uk" };

        public string Run(string email = null, string[] domains = null, string[] secondLevelDomains = null, string[] topLevelDomains = null)
        {
            domains = domains ?? DefaultDomains;
            secondLevelDomains = secondLevelDomains ?? DefaultSecondLevelDomains;
            topLevelDomains = topLevelDomains ?? DefaultTopLevelDomains;

            if (email != null)
            {
                var result = Suggest(EncodeEmail(email), domains, secondLevelDomains, topLevelDomains);

                return result != null ? result.Address : null;
            }

            return null;
        }

        public SplitEmailModel Suggest(string email = null, string[] domains = null, string[] secondLevelDomains = null, string[] topLevelDomains = null, Func<string> distanceFunction = null)
        {
            email = email.ToLower();

            SplitEmailModel emailParts = SplitEmail(email);

            if (emailParts == null)
                return null;

            if (secondLevelDomains != null && topLevelDomains != null)
            {
                // If the email is a valid 2nd-level + top-level, do not suggest anything.
                if (secondLevelDomains.Contains(emailParts.SecondLevelDomain) && topLevelDomains.Contains(emailParts.TopLevelDomain))
                {
                    return null;
                }
            }

            string closestDomain = FindClosestDomain(emailParts.Domain, domains, DomainThreshold);

            if (!String.IsNullOrEmpty(closestDomain))
            {
                if (closestDomain == emailParts.Domain)
                {
                    // The email address exactly matches one of the supplied domains; do not return a suggestion.
                    return null;
                }
                else
                {
                    // The email address closely matches one of the supplied domains; return a suggestion
                    SplitEmailModel result = new SplitEmailModel { Username = emailParts.Username, Domain = closestDomain, Address = emailParts.Username + "@" + closestDomain };

                    return result;
                }
            }

            // The email address does not closely match one of the supplied domains
            var closestSecondLevelDomain = FindClosestDomain(emailParts.SecondLevelDomain, secondLevelDomains, SecondLevelThreshold);
            var closestTopLevelDomain = FindClosestDomain(emailParts.TopLevelDomain, topLevelDomains, TopLevelThreshold);

            if (!String.IsNullOrEmpty(emailParts.Domain))
            {
                closestDomain = emailParts.Domain;
                var rtrn = false;

                if (!String.IsNullOrEmpty(closestSecondLevelDomain) && closestSecondLevelDomain != emailParts.SecondLevelDomain)
                {
                    // The email address may have a mispelled second-level domain; return a suggestion
                    closestDomain = closestDomain.Replace(emailParts.SecondLevelDomain, closestSecondLevelDomain);
                    //closestDomain = closestDomain.Replace(new RegExp(emailParts.topLevelDomain + "$"), closestTopLevelDomain);
                    rtrn = true;
                }

                if (!String.IsNullOrEmpty(closestTopLevelDomain) && closestTopLevelDomain != emailParts.TopLevelDomain && emailParts.SecondLevelDomain != "")
                {
                    // The email address may have a mispelled top-level domain; return a suggestion
                    closestDomain = closestDomain.Replace(emailParts.TopLevelDomain, closestTopLevelDomain);
                    rtrn = true;
                }

                if (rtrn)
                {
                    var result = new SplitEmailModel { Username = emailParts.Address, Domain = closestDomain, Address = emailParts.Username + "@" + closestDomain };
                    return result;
                }
            }

            /* The email address exactly matches one of the supplied domains, does not closely
             * match any domain and does not appear to simply have a mispelled top-level domain,
             * or is an invalid email address; do not return a suggestion.
             */
            return null;
        }

        public string FindClosestDomain(string domain = null, string[] domains = null, int threshold = 0)
        {
            threshold = threshold > 0 ? threshold : TopLevelThreshold;
            double dist;
            double minDist = double.PositiveInfinity;
            string closestDomain = "com";

            if (String.IsNullOrEmpty(domain) || domains.Length <= 0)
            {
                return null;
            }

            for (var i = 0; i < domains.Length; i++)
            {
                if (domain == domains[i])
                {
                    return domain;
                }
                dist = Sift4.SimplestDistance(domain, domains[i], threshold); 
                if (dist < minDist)
                {
                    minDist = dist;
                    closestDomain = domains[i];
                }
            }

            if (minDist <= threshold && !String.IsNullOrEmpty(closestDomain))
            {
                return closestDomain;
            }
            else
            {
                return null;
            }
        }

        // If this returns null, it failed.
        public SplitEmailModel SplitEmail(string email = null)
        {
            if (String.IsNullOrEmpty(email))
                return null;

            email = !String.IsNullOrEmpty(email) ? email.Trim() : null; // trim() not exist in old IE!
            string[] parts = new string[] { };
            parts = email.Split('@');

            if (parts.Length < 2)
            {
                return null;
            }

            for (var i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "")
                {
                    return null;
                }
            }

            string username = parts.First();
            string domain = parts.Last();
            string[] domainParts = domain.Split('.');
            string sld = "";
            string tld = "";

            if (domainParts.Length == 0)
            {
                // The address does not have a top-level domain
                return null;
            }
            else if (domainParts.Length == 1)
            {
                // The address has only a top-level domain (valid under RFC)
                tld = domainParts[0];
            }
            else
            {
                // The address has a domain and a top-level domain
                sld = domainParts[0];
                for (var j = 1; j < domainParts.Length; j++)
                {
                    tld += domainParts[j] + '.';
                }
                tld = tld.Substring(0, tld.Length - 1);
            }

            SplitEmailModel result = new SplitEmailModel { TopLevelDomain = tld, SecondLevelDomain = sld, Username = username, Domain = domain, Address = String.Join("@", parts) };

            return result;
        }

        // Encode the email address to prevent XSS but leave in valid
        // characters, following this official spec:
        // http://en.wikipedia.org/wiki/Email_address#Syntax
        public string EncodeEmail(string email)
        {
            var result = Uri.EscapeUriString(email);
            result = result.Replace("%20", " ").Replace("%25", "%").Replace("%5E", "^")
                           .Replace("%60", "`").Replace("%7B", "{").Replace("%7C", "|")
                           .Replace("%7D", "}");
            return result;
        }
    }
}

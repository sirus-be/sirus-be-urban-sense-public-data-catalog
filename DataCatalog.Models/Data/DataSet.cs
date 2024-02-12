using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataCatalog.Models
{
    [Display(Name = "A collection of data, published or curated by a single agent, and available for access or download in one or more representations.")]
    public class DataSet
    {
        [JsonPropertyName("identifier")]
        [JsonProperty("identifier")]
        [Display(Name = "A unique identifier of the item.")]
        public string Identifier { get; set; }

        [JsonPropertyName("@type")]
        [JsonProperty("@type")]
        public string Type { get; } = "dcat:Dataset";

        [JsonPropertyName("title")]
        [JsonProperty("title")]
        [Display(Name ="A name given to the dataset.")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        [JsonProperty("description")]
        [Display(Name = "A free-text account of the dataset.")]
        public string Description { get; set; }

        [JsonPropertyName("rights")]
        [JsonProperty("rights")]
        [Display(Name = "A statement that concerns all rights not addressed with dcterms:license or dcterms:accessRights, such as copyright statements.")]
        public string Rights { get; set; }

        [JsonPropertyName("issued")]
        [JsonProperty("issued")]
        [Display(Name = "Release date. Date of formal issuance (e.g., publication) of the dataset.")]
        public string Issued { get; set; }

        [JsonPropertyName("modified")]
        [JsonProperty("modified")]
        [Display(Name = "Update/modification date. Most recent date on which the dataset was changed, updated or modified.")]
        public string Modified { get; set; }

        [JsonPropertyName("keyword")]
        [JsonProperty("keyword")]
        [Display(Name = "A keyword or tag describing the dataset.")]
        public List<string> Keyword { get; set; } = new List<string>();

        [JsonPropertyName("theme")]
        [JsonProperty("theme")]
        [Display(Name = "A main category of the resource. A resource can have multiple themes.")]
        public List<string> Theme { get; set; } = new List<string>();

        [JsonPropertyName("publisher")]
        [JsonProperty("publisher")]
        [Display(Name = "An entity responsible for making the dataset available.")]
        public Organization Publisher { get; set; } = new Organization();

        [JsonPropertyName("contactPoint")]
        [JsonProperty("contactPoint")]
        [Display(Name = "Link a dataset to relevant contact information which is provided using VCard")]
        public ContactPoint ContactPoint { get; set; } = new ContactPoint();

        [JsonPropertyName("distribution")]
        [JsonProperty("distribution")]
        public List<Distribution> Distribution { get; set; } = new List<Distribution>();

        [JsonPropertyName("accrualPeriodicity")]
        [JsonProperty("accrualPeriodicity")]
        [Display(Name = "The frequency at which dataset is published.")]
        public string AccrualPeriodicity { get; set; }

        [JsonPropertyName("spatial")]
        [JsonProperty("spatial")]
        [Display(Name = "The geographical area covered by the dataset.")]
        public string Spatial { get; set; }

        [JsonPropertyName("temporalResolution")]
        [JsonProperty("temporalResolution")]
        [Display(Name = "Minimum time period resolvable in the dataset.")]
        public string TemporalResolution { get; set; }

        [JsonPropertyName("accessRights")]
        [JsonProperty("accessRights")]
        [Display(Name = "Information about who can access the resource or an indication of its security status.")]
        public string AccessRights { get; set; }

        [JsonPropertyName("conformsTo")]
        [JsonProperty("conformsTo")]
        [Display(Name = "An established standard to which the described resource conforms.")]
        public string ConformsTo { get; set; }

        [JsonPropertyName("language")]
        [JsonProperty("language")]
        [Display(Name = "The language of the dataset. If a ISO 639-1 (two-letter) code is defined for language, then its corresponding IRI should be used; if no ISO 639-1 code is defined, then IRI corresponding to the ISO 639-2 (three-letter) code should be used.")]
        public string Language { get; set; }

        [JsonPropertyName("license")]
        [JsonProperty("license")]
        [Display(Name = "A legal document under which the resource is made available.")]
        public string License { get; set; }

        [JsonPropertyName("isReferencedBy")]
        [JsonProperty("isReferencedBy")]
        [Display(Name = "A related resource, such as a publication, that references, cites, or otherwise points to the cataloged resource.")]
        public string IsReferencedBy { get; set; }

        [JsonPropertyName("replaces")]
        [JsonProperty("replaces")]
        [Display(Name = "A related resource that is supplanted, displaced, or superseded by the described resource.")]
        public string Replaces { get; set; }

        [JsonPropertyName("version")]
        [JsonProperty("version")]
        [Display(Name = "The version number of a dataset.")]
        public string Version { get; set; }

        [JsonPropertyName("status")]
        [JsonProperty("status")]
        [Display(Name = "The status of the dataset in the context of a particular workflow process.")]
        public string Status { get; set; }

        [JsonPropertyName("landingsPage")]
        [JsonProperty("landingsPage")]
        [Display(Name = "A Web page that can be navigated to in a Web browser to gain access to the dataset and/or additional information.")]
        public string LandingsPage { get; set; }

        [JsonPropertyName("creator")]
        [JsonProperty("creator")]
        [Display(Name = "The entity responsible for producing the resource.")]
        public Organization Creator { get; set; }

        [JsonPropertyName("containsPersonalData")]
        [JsonProperty("containsPersonalData")]
        [Display(Name = "The dataset contains personal data.")]
        public bool ContainsPersonalData { get; set; }
    }

    [Display(Name = "An organization.")]
    public class Organization
    {
        [JsonPropertyName("@type")]
        [JsonProperty("@type")]
        public string Type { get; } = "foaf:Organization";

        [JsonPropertyName("name")]
        [JsonProperty("name")]
        [Display(Name = "Organization's name")]
        public string Name { get; set; }

        [JsonPropertyName("homepage")]
        [JsonProperty("homepage")]
        [Display(Name = "Organization's homepage")]
        public string Homepage { get; set; }
    }

    [Display(Name = "A person.")]
    public class Person
    {
        [JsonPropertyName("@type")]
        [JsonProperty("@type")]
        public string Type { get; } = "foaf:Person";

        [JsonPropertyName("name")]
        [JsonProperty("name")]
        [Display(Name = "Person's name")]
        public string Name { get; set; }
    }

    public class Page
    {
        [JsonPropertyName("@type")]
        [JsonProperty("@type")]
        public string Type { get; } = "foaf:Page";

        [JsonPropertyName("homepage")]
        [JsonProperty("homepage")]
        [Display(Name = "Homepage")]
        public string Homepage { get; set; }
    }

    public class ContactPoint
    {
        [JsonPropertyName("@type")]
        [JsonProperty("@type")]
        public string Type { get; } = "vcard:Contact";

        [JsonPropertyName("fn")]
        [JsonProperty("fn")]
        [Display(Name = "The full name of the contact.")]
        public string Fn { get; set; }

        [JsonPropertyName("hasEmail")]
        [JsonProperty("hasEmail")]
        [Display(Name = "The email address as a mailto URI.")]
        public string HasEmail { get; set; }

        [JsonPropertyName("hasTelephone")]
        [JsonProperty("hasTelephone")]
        [Display(Name = "The telephone number as a tel URI.")]
        public string HasTelephone { get; set; }

        [JsonPropertyName("language")]
        [JsonProperty("language")]
        [Display(Name = "The language of the object.")]
        public string Language { get; set; }
    }

    [Display(Name = "Represents an accessible form of a dataset such as an Api or downloadable file.")]
    public class Distribution
    {
        [JsonPropertyName("@type")]
        [JsonProperty("@type")]
        public string Type { get; } = "dcat:Distribution";

        [JsonPropertyName("title")]
        [JsonProperty("title")]
        [Display(Name = "A name given to the distribution.")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        [JsonProperty("description")]
        [Display(Name = "free-text account of the distribution.")]
        public string Description { get; set; }

        [JsonPropertyName("accessURL")]
        [JsonProperty("accessURL")]
        [Display(Name = "A URL of the resource that gives access to a distribution of the dataset.")]
        public string AccessURL { get; set; }

        [JsonPropertyName("format")]
        [JsonProperty("format")]
        [Display(Name = "The file format of the distribution.")]
        public string Format { get; set; }

        [JsonPropertyName("mediaType")]
        [JsonProperty("mediaType")]
        [Display(Name = "The media type of the distribution.")]
        public string MediaType { get; set; }

    }
}

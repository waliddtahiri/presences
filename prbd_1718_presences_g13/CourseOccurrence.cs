//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace prbd_1718_presences_g13
{
    using System;
    using System.Collections.Generic;
    
    public partial class CourseOccurrence
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseOccurrence()
        {
            this.Presence = new HashSet<Presence>();
        }
    
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Course Course { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presence> Presence { get; set; }
    }
}
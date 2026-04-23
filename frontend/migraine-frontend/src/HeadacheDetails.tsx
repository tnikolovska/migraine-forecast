import { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

interface Symptom {
  id: number;
  name: string;
  description: string;
  type: number; 
}

interface HealthCondition {
  id: number;
  name: string;
  description: string;
  symptoms: Symptom[];
}

const HeadacheDetails = () => {
  const [conditions, setConditions] = useState<HealthCondition[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Promenio sam URL da sklonim /1 ako želiš da povučeš sve iz baze
    // Ako želiš samo jedan, vrati /1 na kraj
    axios.get('http://localhost:5000/api/healthcondition/1')
      .then(res => {
        const data = Array.isArray(res.data) ? res.data : [res.data];
        setConditions(data);
        setLoading(false);
      })
      .catch(err => {
        console.error("Error fetching health details:", err);
        setLoading(false);
      });
  }, []);

  const getPhaseName = (type: number) => {
    const phases = ["Prodrome (Warning)", "Aura (Signal)", "Attack (Peak)", "Postdrome (Hangover)"];
    return phases[type] || "Unknown Phase";
  };

  const getPhaseColor = (type: number) => {
    const colors = ["info", "primary", "danger", "success"];
    return colors[type] || "secondary";
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-5">
      <header className="text-center mb-5">
        <h1 className="display-5 fw-bold text-dark">Migraine Clinical Details</h1>
        <p className="lead text-muted">Deep dive into symptoms and migraine phases.</p>
        <div className="mx-auto bg-primary" style={{ height: '3px', width: '60px' }}></div>
      </header>

      {conditions.map((condition) => (
        <div key={condition.id} className="card border-0 shadow-lg rounded-4 mb-5">
          {/* EDIT DUGME JE SADA OVDE U HEADERU - MNOGO ČISTIJE */}
          <div className="card-header bg-white border-0 pt-4 px-4 d-flex justify-content-between align-items-start">
            <div>
              <h2 className="h3 fw-bold text-primary mb-2">{condition.name}</h2>
              <p className="text-muted mb-0">{condition.description}</p>
            </div>
            <Link to={`/admin/update-condition/1`} className="btn btn-sm btn-outline-secondary rounded-pill px-3 shadow-sm">
              Edit ✏️
            </Link>
          </div>
          
          <div className="card-body p-4">
            <div className="row g-4">
              {[0, 1, 2, 3].map((phaseType) => {
                const filteredSymptoms = condition.symptoms.filter(s => s.type === phaseType);
                if (filteredSymptoms.length === 0) return null;

                return (
                  <div key={phaseType} className="col-12 col-md-6">
                    <div className={`h-100 p-4 rounded-4 border-start border-5 border-${getPhaseColor(phaseType)} bg-light`}>
                      <h4 className={`text-${getPhaseColor(phaseType)} fw-bold mb-3`}>
                        {getPhaseName(phaseType)}
                      </h4>
                      <div className="list-group list-group-flush bg-transparent">
                        {filteredSymptoms.map(s => (
                          <div key={s.id} className="list-group-item bg-transparent border-0 px-0 pb-3">
                            <h6 className="fw-bold mb-1">{s.name}</h6>
                            <p className="small text-muted mb-0">{s.description}</p>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default HeadacheDetails;
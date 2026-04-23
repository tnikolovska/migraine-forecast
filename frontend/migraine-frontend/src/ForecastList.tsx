import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from "react-router-dom";
import type { AxiosResponse, AxiosError } from 'axios';

interface Symptom {
  id:number;
  name: string;
  description: string;
  healthConditionId: number;
  type: string | number;
}

const ForecastList = () => {
  const [items, setItems] = useState<Symptom[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
   const navigate = useNavigate();

  useEffect(() => {
    let isMounted = true;
    axios.get('http://localhost:5000/api/symptom')
      .then((response: AxiosResponse) => {
        if (isMounted) {
          setItems(response.data);
          setLoading(false);
        }
      })
      .catch((err: AxiosError) => {
        if (isMounted) {
          setError(err.message);
          setLoading(false);
        }
      });
    return () => { isMounted = false; };
  }, []);

  // Mapiranje tipova (ako tvoj backend šalje brojeve)
  const formatType = (type: string | number) => {
    const types = ["Prodrome", "Aura", "Attack", "Postdrome"];
    return typeof type === 'number' ? types[type] : type;
  };

  // Filtriranje za "fancy" pretragu
  const filteredItems = items.filter(item => 
    item.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="d-flex justify-content-center py-5">
        <div className="spinner-grow text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-5">
      {/* Header Sekcija */}
      <div className="row mb-5 align-items-center">
        <div className="col-md-6 text-start">
          <h1 className="fw-bold text-dark mb-0">Symptom Directory</h1>
          <p className="text-muted">A comprehensive guide to migraine-related indicators.</p>
        </div>
        <div className="col-md-6">
          <div className="input-group shadow-sm rounded-pill overflow-hidden">
            <span className="input-group-text bg-white border-0 ps-4">🔍</span>
            <input 
              type="text" 
              className="form-control border-0 py-3" 
              placeholder="Search symptoms..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>
      </div>

      {error && (
        <div className="alert alert-danger rounded-4 shadow-sm" role="alert">
          {error}
        </div>
      )}

      {/* Tabela sa Simptomima */}
      <div className="card border-0 shadow-lg rounded-4 overflow-hidden">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead className="bg-light">
              <tr>
                <th className="px-4 py-3 border-0">Symptom Name</th>
                <th className="py-3 border-0">Phase/Type</th>
                <th className="py-3 border-0">Clinical Description</th>
                <th className="py-3 border-0">Edit Symptom</th>
              </tr>
            </thead>
            <tbody>
              {filteredItems.length === 0 ? (
                <tr>
                  <td colSpan={4} className="text-center py-5 text-muted">
                    No symptoms match your search.
                  </td>
                </tr>
              ) : (
                filteredItems.map((symptom, index) => (
                  <tr key={index}>
                    <td className="px-4 py-3">
                      <span className="fw-bold text-primary">{symptom.name}</span>
                    </td>
                    <td>
                      <span className={`badge rounded-pill bg-soft-primary text-primary px-3 py-2`} 
                            style={{ backgroundColor: '#e7f1ff' }}>
                        {formatType(symptom.type)}
                      </span>
                    </td>
                    <td className="text-muted w-50">{symptom.description}</td>
                    <td className="px-4 py-3">
                      <button
                      className="btn btn-primary btn-sm"
                     
                      onClick={() => navigate(`/admin/update-symptom/${symptom.id}`)}
                    >
                      Edit
                    </button>

                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default ForecastList;
import { useState } from 'react';
import axios from 'axios';

export const CreateHealthCondition = () => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    const newCondition = {
      name,
      description
    };

    try {
      // Proveri da li je ruta /api/healthcondition ili /api/healthconditions (množina)
      //await axios.post('http://localhost:5000/api/healthcondition', newCondition);
      //await axios.post('/api/healthcondition', newCondition);
      const token = localStorage.getItem("token");
      await axios.post(
      "/api/healthcondition",newCondition,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
      alert("Health Condition created successfully! 🏥");
      setName('');
      setDescription('');
    } catch (err) {
      console.error("Error creating condition:", err);
      alert("Failed to create condition. Check console.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-12 col-md-8 col-lg-5">
          {/* border-start i border-primary daju onu plavu liniju sa leve strane kao u tvom originalu */}
          <div className="card shadow-lg border-0 border-start border-5 border-primary rounded-4">
            <div className="card-body p-4 p-md-5">
              <div className="text-center mb-4">
                <h2 className="fw-bold text-dark">Create Health Condition 🏥</h2>
                <p className="text-muted">Define a new medical profile for users.</p>
              </div>

              <form onSubmit={handleSubmit}>
                {/* Condition Name */}
                <div className="mb-4">
                  <label className="form-label fw-bold">Condition Name</label>
                  <input 
                    type="text" 
                    className="form-control form-control-lg shadow-sm"
                    placeholder="e.g., Hemiplegic Migraine"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required 
                  />
                </div>

                {/* Condition Description */}
                <div className="mb-4">
                  <label className="form-label fw-bold">Condition Description</label>
                  <textarea 
                    className="form-control shadow-sm"
                    rows={4}
                    placeholder="Describe the clinical characteristics..."
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                  ></textarea>
                </div>

                {/* Submit Button */}
                <div className="d-grid">
                  <button 
                    type="submit" 
                    className="btn btn-primary btn-lg rounded-pill shadow-sm fw-bold"
                    disabled={isSubmitting}
                  >
                    {isSubmitting ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2"></span>
                        Registering...
                      </>
                    ) : (
                      'Register Condition'
                    )}
                  </button>
                </div>
              </form>
            </div>
          </div>
          
          {/* Back button (Opciono) */}
          <div className="text-center mt-4">
            <small className="text-muted">
              Need to add symptoms instead? <a href="/admin/create-symptom" className="text-decoration-none">Click here</a>
            </small>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateHealthCondition;
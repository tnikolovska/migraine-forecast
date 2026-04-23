import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';

export const UpdateHealthCondition = () => {
  const { id } = useParams(); // Gets the ID from the URL
  //const { id } =1;
  const navigate = useNavigate();
  
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // 1. Fetch the existing data when the page loads
  useEffect(() => {
    axios.get(`http://localhost:5000/api/healthcondition/${id}`)
      .then(res => {
        setName(res.data.name);
        setDescription(res.data.description);
        setLoading(false);
      })
      .catch(err => {
        console.error("Error loading condition:", err);
        alert("Condition not found!");
        navigate('/'); // Redirect if error
      });
  }, [id, navigate]);

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    const updatedCondition = {
      id: Number(id),
      name,
      description
    };

    try {
      await axios.put(`http://localhost:5000/api/healthcondition/${id}`, updatedCondition);
      alert("Condition updated successfully! 🔄");
      navigate('/migraineHeadacheDetails'); // Redirect back to details page
    } catch (err) {
      console.error("Update failed:", err);
      alert("Failed to update condition.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (loading) return <div className="text-center py-5"><div className="spinner-border text-primary"></div></div>;

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-12 col-md-8 col-lg-5">
          <div className="card shadow-lg border-0 border-start border-5 border-info rounded-4">
            <div className="card-body p-4 p-md-5">
              <h2 className="fw-bold text-dark mb-4">Update Condition 🛠️</h2>
              
              <form onSubmit={handleUpdate}>
                <div className="mb-4">
                  <label className="form-label fw-bold small text-uppercase text-muted">Condition Name</label>
                  <input 
                    type="text" 
                    className="form-control form-control-lg bg-light"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required 
                  />
                </div>

                <div className="mb-4">
                  <label className="form-label fw-bold small text-uppercase text-muted">Description</label>
                  <textarea 
                    className="form-control bg-light"
                    rows={5}
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                  ></textarea>
                </div>

                <div className="d-grid gap-2">
                  <button 
                    type="submit" 
                    className="btn btn-info btn-lg rounded-pill text-white fw-bold shadow-sm"
                    disabled={isSubmitting}
                  >
                    {isSubmitting ? 'Saving Changes...' : 'Update Details'}
                  </button>
                  <button 
                    type="button" 
                    className="btn btn-link text-muted"
                    onClick={() => navigate(-1)}
                  >
                    Cancel
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UpdateHealthCondition;
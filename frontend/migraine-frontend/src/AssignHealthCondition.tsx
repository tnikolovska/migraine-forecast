import { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

interface HealthCondition {
  id: number;
  name: string;
}

const AssignHealthCondition = () => {
  const [conditions, setConditions] = useState<HealthCondition[]>([]);
  const [selectedId, setSelectedId] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(true);
  const [submitting, setSubmitting] = useState<boolean>(false);
  const navigate = useNavigate();
  // Load conditions
  useEffect(() => {
    const fetchConditions = async () => {
      try {
        const res = await axios.get<HealthCondition[]>(
          //"http://localhost:5000/api/healthcondition"
          "/api/healthcondition"
        );

        setConditions(res.data);
      } catch (err) {
        console.error("Error loading conditions", err);
      } finally {
        setLoading(false);
      }
    };

    fetchConditions();
  }, []);

  // Assign condition
  const handleAssign = async () => {
    if (!selectedId) {
      alert("Please select a health condition");
      return;
    }

    const confirmAssign = window.confirm(
      "Are you sure you have this health condition?"
    );

    if (!confirmAssign) return;

    setSubmitting(true);

    try {
      const token = localStorage.getItem("token");

      await axios.post(
        //"http://localhost:5000/api/userhealthcondition",
        "/api/userhealthcondition",
        {
          healthConditionId: selectedId
        },
        {
          headers: {
            Authorization: `Bearer ${token}`
          }
        }
      );

      alert("Health condition assigned successfully!");
      navigate("/userSymptomSelection");
      setSelectedId(0);
    } catch (err) {
      console.error("Assign failed", err);
      alert("Failed to assign condition");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="container py-5">Loading...</div>;
  }

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6">

          <div className="card shadow p-4 rounded-4">

            <h3 className="text-center mb-3">
              Assign Health Condition
            </h3>

            <p className="text-muted text-center mb-4">
              Select your health condition and confirm assignment.
            </p>

            {/* Dropdown */}
            <div className="mb-3">
              <label className="form-label fw-semibold">
                Health Condition
              </label>

              <select
                className="form-select"
                value={selectedId}
                onChange={(e) => setSelectedId(Number(e.target.value))}
              >
                <option value={0}>-- Select Condition --</option>

                {conditions.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Button */}
            <button
              className="btn btn-primary w-100 rounded-pill"
              onClick={handleAssign}
              disabled={submitting}
            >
              {submitting ? "Assigning..." : "Assign Condition"}
            </button>

          </div>

        </div>
      </div>
    </div>
  );
};

export default AssignHealthCondition;
import { useEffect, useMemo, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

interface Symptom {
  id: number;
  name: string;
  description: string;
  type: string; // Migraine phase string from backend
}

interface GroupedSymptoms {
  [key: string]: Symptom[];
}



const UserSymptomSelection = () => {
  const [symptoms, setSymptoms] = useState<Symptom[]>([]);
  const [selected, setSelected] = useState<number[]>([]);
  const [selectedPhase, setSelectedPhase] = useState<string | null>(null);

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const navigate = useNavigate();

  // Load symptoms
  useEffect(() => {
    axios
      //.get<Symptom[]>("http://localhost:5000/api/symptom")
      .get<Symptom[]>("/api/symptom")
      .then((res) => setSymptoms(res.data))
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  }, []);

       


  // Group by migraine phase
  const grouped = useMemo<GroupedSymptoms>(() => {
    return symptoms.reduce((acc, symptom) => {
      if (!acc[symptom.type]) {
        acc[symptom.type] = [];
      }
      acc[symptom.type].push(symptom);
      return acc;
    }, {} as GroupedSymptoms);
  }, [symptoms]);

  // Toggle checkbox
  /*const toggle = (id: number) => {
    setSelected((prev) =>
      prev.includes(id)
        ? prev.filter((x) => x !== id)
        : [...prev, id]
    );
  };*/

  const toggle = (symptomId: number, phase: string) => {
  // If no phase selected yet → lock phase
  if (!selectedPhase) {
    setSelectedPhase(phase);
  }

  // If user tries to select different phase → block it
  if (selectedPhase && selectedPhase !== phase) {
    alert(`You can only select symptoms from: ${selectedPhase}`);
    return;
  }

  setSelected((prev) =>
    prev.includes(symptomId)
      ? prev.filter((x) => x !== symptomId)
      : [...prev, symptomId]
  );
};


  // Submit selection
  const handleSubmit = async () => {
    if (selected.length === 0) {
      alert("Please select at least one symptom");
      return;
    }

    const confirm = window.confirm(
      "Confirm your symptom selection?"
    );

    if (!confirm) return;

    setSubmitting(true);

      try {
        const token = localStorage.getItem("token");

        await axios.post(
          //"http://localhost:5000/api/usersymptomselection",
           "/api/usersymptomselection",
          { symptomIds: selected },
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        );

        //alert("Symptoms saved successfully!");

        // ✅ IMPORTANT: navigate LAST and DO NOTHING after it
        console.log("Navigating now...");
        navigate("/forecast", { replace: true });

      } catch (err) {
        console.error(err);
        alert("Failed to save symptoms");
      } finally {
        setSubmitting(false);
      }
    }

  if (loading) {
    return <div className="container py-5">Loading symptoms...</div>;
  }

  return (
    <div className="container py-5">

      {/* HEADER */}
      <div className="text-center mb-5">
        <h2 className="fw-bold">Select Your Symptoms</h2>
        <p className="text-muted">
          Choose symptoms you experience during different migraine phases
        </p>
      </div>

       {selectedPhase && (
      <div className="alert alert-info text-center">
        Selected Phase: <strong>{selectedPhase}</strong>
      </div>
    )}

                <button
        className="btn btn-outline-secondary btn-sm mb-3"
        onClick={() => {
            setSelected([]);
            setSelectedPhase(null);
        }}
        >
        Reset Selection
        </button>


      {/* GROUPED SYMPTOMS */}
      {Object.entries(grouped).map(([phase, items]) => (
        <div key={phase} className="mb-4">

          {/* Phase Header */}
          <div className="bg-primary text-white p-2 px-3 rounded-3 mb-3 shadow-sm">
            {phase}
          </div>

          <div className="row g-3">
            {items.map((symptom) => (
              <div key={symptom.id} className="col-md-6 col-lg-4">

                <div className="card shadow-sm border-0 rounded-4 h-100">

                  <div className="card-body d-flex align-items-start gap-3">

                    {/* Checkbox */}
                    <input
                      type="checkbox"
                      className="form-check-input mt-1"
                      checked={selected.includes(symptom.id)}
                      onChange={() => toggle(symptom.id, symptom.type)}
                    />

                    {/* Content */}
                    <div>
                      <h6 className="fw-semibold mb-1">
                        {symptom.name}
                      </h6>

                      <small className="text-muted">
                        {symptom.description}
                      </small>
                    </div>

                  </div>
                </div>

              </div>
            ))}
          </div>

        </div>
      ))}

      {/* SUBMIT BUTTON */}
      <div className="text-center mt-5">
        <button
          type="button"
          className="btn btn-primary btn-lg px-5 rounded-pill shadow"
          onClick={handleSubmit}
          disabled={submitting}
        >
          {submitting ? "Saving..." : "Save My Symptoms"}
        </button>
      </div>

    </div>
  );
};

export default UserSymptomSelection;
import React, { useEffect, useState } from "react";
import { fetchRebellion } from "app/domain/services/rebellion";
import PropTypes from "prop-types";

import RebellionDetail from "app/components/rebellion-detail";

const Rebellion = ({ match }) => {
	const { rebellionId } = match.params;
	const [rebellion, setRebellion] = useState();

	useEffect(() => {
		const load = async () => {
			setRebellion(await fetchRebellion(rebellionId));
		};
		load();
	}, [rebellionId]);

	if (!rebellion) {
		return <div>Loading</div>;
	}

	console.log(rebellion);
	return (
		<div>
			<RebellionDetail rebellion={rebellion} />
		</div>
	);
};

Rebellion.propTypes = {
	getRebellions: PropTypes.func,
	rebellions: PropTypes.array
};

export default Rebellion;

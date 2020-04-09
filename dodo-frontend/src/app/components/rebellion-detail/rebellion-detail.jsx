import React from "react"
import { Link } from "react-router-dom"

import { PageTitle } from "app/components/page-title"
import { DateLayout } from "app/components/date-layout"
import { Button } from "app/components/button"
import styles from "./rebellion-detail.module.scss"

export const RebellionDetail = ({ rebellion }) => {
	return (
		<DateLayout
			startDate={
				rebellion.StartDate ? new Date(rebellion.StartDate) : null
			}
			endDate={rebellion.EndDate ? new Date(rebellion.EndDate) : null}
			title={<PageTitle title={rebellion.Name} subTitle="Glasgow" />}
		>
			<div className={styles.detail}>{rebellion.PublicDescription}</div>
			<div className={styles.join}>
				<Button
					as={<Link to={"/rebellion/join"} />}
					style={{ display: "inline-block" }}
				>
					JOIN REBELLION
				</Button>
			</div>
		</DateLayout>
	)
}
